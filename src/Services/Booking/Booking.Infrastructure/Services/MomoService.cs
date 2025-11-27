using Booking.Application.Interfaces;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using Booking.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Booking.Infrastructure.Services
{
    public class MomoService : IMomoService
    {
        private readonly MomoSettings _settings;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<MomoService> _logger;
        private readonly HttpClient _httpClient;

        public MomoService(
            IOptions<MomoSettings> settings,
            IPaymentRepository paymentRepository,
            ILogger<MomoService> logger,
            HttpClient httpClient)
        {
            _settings = settings.Value;
            _paymentRepository = paymentRepository;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<(bool success, string paymentUrl, string message)> CreatePaymentUrlAsync(
            Guid bookingId,
            decimal amount,
            string bookingCode,
            string ipAddress)
        {
            try
            {
                var requestId = Guid.NewGuid().ToString();
                var orderId = bookingId.ToString();
                var orderInfo = $"Thanh toan booking {bookingCode}";
                var amountLong = (long)amount;

                var rawSignature = $"accessKey={_settings.AccessKey}" +
                                 $"&amount={amountLong}" +
                                 $"&extraData=" +
                                 $"&ipnUrl={_settings.IpnUrl}" +
                                 $"&orderId={orderId}" +
                                 $"&orderInfo={orderInfo}" +
                                 $"&partnerCode={_settings.PartnerCode}" +
                                 $"&redirectUrl={_settings.ReturnUrl}" +
                                 $"&requestId={requestId}" +
                                 $"&requestType=captureWallet";

                var signature = ComputeHmacSha256(rawSignature, _settings.SecretKey);
                var requestData = new
                {
                    partnerCode = _settings.PartnerCode,
                    accessKey = _settings.AccessKey,
                    requestId,
                    amount = amountLong,
                    orderId,
                    orderInfo,
                    redirectUrl = _settings.ReturnUrl,
                    ipnUrl = _settings.IpnUrl,
                    extraData = "",
                    requestType = "captureWallet",
                    signature,
                    lang = "vi"
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Creating Momo payment for booking {BookingId}, amount: {Amount}", bookingId, amount);
                _logger.LogInformation("Momo Request: {Request}", jsonContent);
                
                var response = await _httpClient.PostAsync(_settings.Endpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Momo Response Status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Momo Response Body: {ResponseBody}", responseBody);
                
                if (!response.IsSuccessStatusCode || responseBody.StartsWith("<"))
                {
                    _logger.LogError("Momo API returned non-JSON response: {ResponseBody}", responseBody);
                    return (false, string.Empty, "Momo API returned invalid response");
                }
                
                var momoResponse = JsonSerializer.Deserialize<MomoPaymentResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (momoResponse?.ResultCode == 0)
                {
                    var paymentSpec = new PaymentByBookingIdSpecification(bookingId);
                    var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);
                    if (payment != null)
                    {
                        payment.TransactionId = requestId;
                        payment.PaymentUrl = momoResponse.PayUrl;
                        _paymentRepository.Update(payment);
                    }

                    _logger.LogInformation("Momo payment URL created successfully for booking {BookingId}", bookingId);
                    return (true, momoResponse.PayUrl ?? string.Empty, "Payment URL created successfully");
                }

                _logger.LogWarning("Momo payment creation failed. Result code: {ResultCode}, Message: {Message}",
                    momoResponse?.ResultCode, momoResponse?.Message);

                return (false, string.Empty, momoResponse?.Message ?? "Failed to create payment URL");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Momo payment URL for booking {BookingId}", bookingId);
                return (false, string.Empty, $"Error: {ex.Message}");
            }
        }

        public Task<(bool isValid, string message, string transactionId)> ValidateCallbackAsync(
            Dictionary<string, string> queryParams)
        {
            try
            {
                var signature = queryParams.GetValueOrDefault("signature", "");
                var orderId = queryParams.GetValueOrDefault("orderId", "");
                var resultCode = int.Parse(queryParams.GetValueOrDefault("resultCode", "-1"));
                var transId = queryParams.GetValueOrDefault("transId", "");
                var message = queryParams.GetValueOrDefault("message", "");

                var rawSignature = $"accessKey={queryParams["accessKey"]}" +
                                 $"&amount={queryParams["amount"]}" +
                                 $"&extraData={queryParams["extraData"]}" +
                                 $"&message={message}" +
                                 $"&orderId={orderId}" +
                                 $"&orderInfo={queryParams["orderInfo"]}" +
                                 $"&orderType={queryParams["orderType"]}" +
                                 $"&partnerCode={queryParams["partnerCode"]}" +
                                 $"&payType={queryParams["payType"]}" +
                                 $"&requestId={queryParams["requestId"]}" +
                                 $"&responseTime={queryParams["responseTime"]}" +
                                 $"&resultCode={resultCode}" +
                                 $"&transId={transId}";

                var calculatedSignature = ComputeHmacSha256(rawSignature, _settings.SecretKey);

                if (signature != calculatedSignature)
                {
                    _logger.LogWarning("Momo callback signature mismatch for order {OrderId}", orderId);
                    return Task.FromResult((false, "Invalid signature", ""));
                }

                if (resultCode == 0)
                {
                    _logger.LogInformation("Momo payment successful for order {OrderId}, Transaction: {TransId}", orderId, transId);
                    return Task.FromResult((true, "Payment successful", transId));
                }

                _logger.LogWarning("Momo payment failed for order {OrderId}. Result code: {ResultCode}, Message: {Message}",
                    orderId, resultCode, message);

                return Task.FromResult((false, message, transId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Momo callback");
                return Task.FromResult((false, $"Error: {ex.Message}", ""));
            }
        }

        public async Task<(bool success, string message, string refundId)> ProcessRefundAsync(
            Guid paymentId,
            decimal refundAmount,
            string reason)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return (false, "Payment not found", "");
                }

                var requestId = Guid.NewGuid().ToString();
                var orderId = payment.BookingId.ToString();
                var transId = payment.TransactionId ?? "";
                var amountLong = (long)refundAmount;

                var rawSignature = $"accessKey={_settings.AccessKey}" +
                                 $"&amount={amountLong}" +
                                 $"&description={reason}" +
                                 $"&orderId={orderId}" +
                                 $"&partnerCode={_settings.PartnerCode}" +
                                 $"&requestId={requestId}" +
                                 $"&transId={transId}";

                var signature = ComputeHmacSha256(rawSignature, _settings.SecretKey);

                var requestData = new
                {
                    partnerCode = _settings.PartnerCode,
                    accessKey = _settings.AccessKey,
                    requestId,
                    amount = amountLong,
                    orderId,
                    transId,
                    description = reason,
                    signature,
                    lang = "vi"
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var refundEndpoint = _settings.Endpoint.Replace("/create", "/refund");

                _logger.LogInformation("Processing Momo refund for payment {PaymentId}, amount: {Amount}", paymentId, refundAmount);

                var response = await _httpClient.PostAsync(refundEndpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var momoResponse = JsonSerializer.Deserialize<MomoRefundResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (momoResponse?.ResultCode == 0)
                {
                    _logger.LogInformation("Momo refund successful for payment {PaymentId}, Refund ID: {RefundId}",
                        paymentId, momoResponse.TransId);
                    return (true, "Refund processed successfully", momoResponse.TransId ?? requestId);
                }

                _logger.LogWarning("Momo refund failed. Result code: {ResultCode}, Message: {Message}",
                    momoResponse?.ResultCode, momoResponse?.Message);

                return (false, momoResponse?.Message ?? "Refund failed", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Momo refund for payment {PaymentId}", paymentId);
                return (false, $"Error: {ex.Message}", "");
            }
        }

        private string ComputeHmacSha256(string message, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private class MomoPaymentResponse
        {
            public string? PartnerCode { get; set; }
            public string? RequestId { get; set; }
            public string? OrderId { get; set; }
            public long Amount { get; set; }
            public long ResponseTime { get; set; }
            public string? Message { get; set; }
            public int ResultCode { get; set; }
            public string? PayUrl { get; set; }
        }

        private class MomoRefundResponse
        {
            public string? PartnerCode { get; set; }
            public string? RequestId { get; set; }
            public string? OrderId { get; set; }
            public long Amount { get; set; }
            public string? TransId { get; set; }
            public int ResultCode { get; set; }
            public string? Message { get; set; }
        }
    }
}
