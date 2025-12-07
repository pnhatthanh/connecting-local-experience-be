using Booking.Application.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using BuildingBlocks.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives; 
using Microsoft.Extensions.Logging;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;
using VNPAY.Models.Exceptions;

namespace Booking.Infrastructure.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IVnpayClient _vnpayClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<VnPayService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IVnpayClient vnpayClient,
            IPaymentRepository paymentRepository,
            ILogger<VnPayService> logger, 
            IUnitOfWork unitOfWork)
        {
            _vnpayClient = vnpayClient;
            _paymentRepository = paymentRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool success, string paymentUrl, string transactionId)> CreatePaymentUrlAsync(
            Guid bookingId, decimal amount, string bookingCode)
        {
            try
            {
                var request = new VnpayPaymentRequest
                {
                    Money = (double)amount,
                    Description = $"Thanh toan booking {bookingCode}",
                    BankCode = BankCode.ANY,
                    Language = DisplayLanguage.Vietnamese
                    
                };
                var paymentUrlInfo = _vnpayClient.CreatePaymentUrl(request);
                var paymentUrl = paymentUrlInfo.Url;
                _logger.LogInformation("VNPay URL created | BookingCode: {BookingCode} | VnpTxnRef: {TxnRef} | URL: {Url}", 
                    bookingCode, paymentUrlInfo.PaymentId, paymentUrl);
                return await Task.FromResult((true, paymentUrl, paymentUrlInfo.PaymentId.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay URL for booking {BookingId}", bookingId);
                return (false, "", ex.Message);
            }
        }

        public async Task<(bool isValid, string message, string transactionNo)> ValidateCallbackAsync(IQueryCollection? queryParams)
        {
            try
            {
                if (queryParams == null)
                    return (false, "Query parameters are null", "");
                var paymentResult = _vnpayClient.GetPaymentResult(queryParams);
                _logger.LogInformation("Payment SUCCESS - PaymentId: {PaymentId} | VNPayTxn: {VnpTxn} | Card: {Card}", 
                    paymentResult.PaymentId, paymentResult.VnpayTransactionId, paymentResult.CardType);

                return await Task.FromResult((true, "Payment successful", paymentResult.VnpayTransactionId.ToString()));
            }
            catch (VnpayException ex)
            {
                _logger.LogWarning("Payment FAILED - Code: {Code} | Msg: {Msg}", ex.PaymentResponseCode, ex.Message);
                return (false, ex.Message, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VNPay callback");
                return (false, "Exception: " + ex.Message, "");
            }
        }

        public (bool isValid, string message) ValidateIpnCallback(IQueryCollection? queryParams)
        {
            try
            {
                if (queryParams == null)
                    return (false, "Query parameters are null");
                var paymentResult = _vnpayClient.GetPaymentResult(queryParams);
                _logger.LogInformation("IPN SUCCESS - PaymentId: {PaymentId}", paymentResult.PaymentId);
                return (true, "Confirm Success");
            }
            catch (VnpayException ex)
            {
                _logger.LogWarning("IPN FAILED - Code: {Code} | Msg: {Msg}", ex.PaymentResponseCode, ex.Message);
                return (false, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VNPay IPN");
                return (false, ex.Message);
            }
        }
    }
}
