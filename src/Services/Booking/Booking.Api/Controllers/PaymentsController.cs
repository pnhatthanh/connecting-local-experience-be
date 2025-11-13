using Booking.Application.Interfaces;
using Booking.Domain.Repositories;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IVnPayService vnPayService,
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            ILogger<PaymentsController> logger)
        {
            _vnPayService = vnPayService;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        [HttpPost("{bookingId}/create-payment-url")]
        public async Task<IActionResult> CreatePaymentUrl(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return NotFound(new { success = false, message = "Booking not found" });

            var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);
            if (payment == null)
                return NotFound(new { success = false, message = "Payment not found" });

            if (payment.Status == PaymentStatus.Paid)
                return BadRequest(new { success = false, message = "Booking already paid" });

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var paymentUrl = _vnPayService.CreatePaymentUrl(bookingId, payment.Amount, booking.BookingCode, ipAddress);

            return Ok(new { success = true, data = new { paymentUrl } });
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayCallback()
        {
            try
            {
                var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                var (isValid, responseCode) = await _vnPayService.ValidateCallbackAsync(queryParams);

                if (!isValid || responseCode != "00")
                {
                    _logger.LogWarning("VNPay callback failed. Response code: {ResponseCode}", responseCode);
                    return Redirect($"/payment/failed?code={responseCode}");
                }

                var vnpTxnRef = queryParams.GetValueOrDefault("vnp_TxnRef", "");
                var payment = await _paymentRepository.GetByVnpTxnRefAsync(vnpTxnRef);

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Paid;
                    payment.VnpResponseCode = responseCode;
                    payment.VnpSecureHash = queryParams.GetValueOrDefault("vnp_SecureHash", "");
                    payment.VnpBankCode = queryParams.GetValueOrDefault("vnp_BankCode", "");
                    payment.PaidAt = DateTime.UtcNow;
                    payment.UpdatedAt = DateTime.UtcNow;

                    var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
                    if (booking != null)
                    {
                        booking.Status = BookingStatus.Confirmed;
                        booking.UpdatedAt = DateTime.UtcNow;
                    }

                    await _paymentRepository.UpdateAsync(payment);
                    // Note: Assuming unit of work will save changes
                }

                return Redirect($"/payment/success?bookingCode={payment?.Booking?.BookingCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return Redirect("/payment/failed");
            }
        }
    }
}
