using Booking.Application.Handlers.Commands.ProcessVnPayCallback;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                var queryParams = Request.Query;
                var command = new ProcessVnPayCallbackCommand(queryParams);
                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    if (result.ClientType == "App")
                        return Redirect($"connecting://payment-success?bookingCode={result.BookingCode}");
                    else
                        return Redirect($"https://connecting-local-experience-fe.vercel.app/booking-success?bookingCode={result.BookingCode}");
                }
                return Redirect($"https://connecting-local-experience-fe.vercel.app/booking-failed?message={Uri.EscapeDataString(result.Message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay return");
                return Redirect("https://connecting-local-experience-fe.vercel.app/booking-failed");
            }
        }

        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpn()
        {
            try
            {
                _logger.LogInformation("Received VNPay IPN callback");
                var queryParams = Request.Query;
                
                var command = new ProcessVnPayCallbackCommand(queryParams);
                var result = await _mediator.Send(command);

                _logger.LogInformation("VNPay IPN processed. Success: {Success}, Message: {Message}", 
                    result.Success, result.Message);

                return Ok(new
                {
                    RspCode = result.Success ? "00" : "99",
                    Message = result.Success ? "Confirm Success" : result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay IPN");
                return Ok(new
                {
                    RspCode = "99",
                    Message = $"Error: {ex.Message}"
                });
            }
        }
    }
}
