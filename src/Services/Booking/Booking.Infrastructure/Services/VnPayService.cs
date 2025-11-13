using Booking.Application.Interfaces;
using Booking.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Booking.Infrastructure.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;
        private readonly ILogger<VnPayService> _logger;

        public VnPayService(IOptions<VnPaySettings> settings, ILogger<VnPayService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public string CreatePaymentUrl(Guid bookingId, decimal amount, string bookingCode, string ipAddress)
        {
            var vnpay = new VnPayLibrary();
            var tick = DateTime.UtcNow.Ticks;
            
            vnpay.AddRequestData("vnp_Version", _settings.Version);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan booking {bookingCode}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", tick.ToString());
            
            string paymentUrl = vnpay.CreateRequestUrl(_settings.Url, _settings.HashSecret);
            
            _logger.LogInformation("Created VNPay payment URL for booking {BookingId}", bookingId);
            
            return paymentUrl;
        }

        public async Task<(bool isValid, string responseCode)> ValidateCallbackAsync(Dictionary<string, string> queryParams)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                
                foreach (var (key, value) in queryParams)
                {
                    if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value);
                    }
                }

                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_SecureHash = queryParams.ContainsKey("vnp_SecureHash") ? queryParams["vnp_SecureHash"] : "";
                
                bool isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, _settings.HashSecret);

                if (!isValidSignature)
                {
                    _logger.LogWarning("Invalid VNPay signature");
                    return (false, "97"); // Invalid signature
                }

                await Task.CompletedTask;
                return (vnp_ResponseCode == "00", vnp_ResponseCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VNPay callback");
                return (false, "99");
            }
        }

        public async Task<(bool success, string responseCode)> ProcessRefundAsync(Guid paymentId, decimal refundAmount, string reason)
        {
            try
            {
                // In production, you would call VNPay's refund API here
                // For now, we'll simulate a successful refund
                _logger.LogInformation("Processing refund for payment {PaymentId}, amount {Amount}", paymentId, refundAmount);
                
                await Task.Delay(100); // Simulate API call
                
                // Simulate success
                return (true, "00");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment {PaymentId}", paymentId);
                return (false, "99");
            }
        }
    }

    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out string? value) ? value : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
            }

            string queryString = data.ToString();
            
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            string signData = queryString;
            string vnpSecureHash = HmacSHA512(hashSecret, signData);
            
            return $"{baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHash"))
            {
                data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
            }

            string signData = data.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }

            string myChecksum = HmacSHA512(secretKey, signData);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}
