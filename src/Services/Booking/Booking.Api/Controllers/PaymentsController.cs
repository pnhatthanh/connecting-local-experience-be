using Booking.Application.Handlers.Commands.CreatePaymentUrl;
using Booking.Application.Handlers.Commands.ProcessPaymentCallback;
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

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var command = new CreatePaymentUrlCommand(request.BookingId, ipAddress);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new
            {
                success = true,
                data = new
                {
                    paymentUrl = result.PaymentUrl,
                    provider = result.Provider
                }
            });
        }


        [HttpPost("momo-callback")]
        public async Task<IActionResult> MomoCallback([FromBody] Dictionary<string, object> momoData)
        {
            try
            {
                _logger.LogInformation("Received Momo IPN callback: {Data}", System.Text.Json.JsonSerializer.Serialize(momoData));
                
                // Convert object values to string
                var queryParams = momoData.ToDictionary(
                    x => x.Key, 
                    x => x.Value?.ToString() ?? ""
                );
                
                var command = new ProcessMomoCallbackCommand(queryParams);
                var result = await _mediator.Send(command);

                _logger.LogInformation("Momo IPN processed. Success: {Success}, Message: {Message}", 
                    result.Success, result.Message);

                // MoMo expects HTTP 200 with this exact format
                return Ok(new
                {
                    partnerCode = queryParams.GetValueOrDefault("partnerCode", ""),
                    orderId = queryParams.GetValueOrDefault("orderId", ""),
                    requestId = queryParams.GetValueOrDefault("requestId", ""),
                    amount = long.Parse(queryParams.GetValueOrDefault("amount", "0")),
                    orderInfo = queryParams.GetValueOrDefault("orderInfo", ""),
                    orderType = queryParams.GetValueOrDefault("orderType", ""),
                    transId = long.Parse(queryParams.GetValueOrDefault("transId", "0")),
                    resultCode = result.Success ? 0 : 1,
                    message = result.Success ? "Successful." : result.Message,
                    payType = queryParams.GetValueOrDefault("payType", ""),
                    responseTime = long.Parse(queryParams.GetValueOrDefault("responseTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())),
                    extraData = queryParams.GetValueOrDefault("extraData", ""),
                    signature = queryParams.GetValueOrDefault("signature", "")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Momo callback");
                return StatusCode(200, new
                {
                    resultCode = 1,
                    message = $"Error: {ex.Message}"
                });
            }
        }
        [HttpGet("momo-return")]
        public IActionResult MomoReturn()
        {
            try
            {
                var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                var resultCode = int.Parse(queryParams.GetValueOrDefault("resultCode", "-1"));
                var orderId = queryParams.GetValueOrDefault("orderId", "");

                if (resultCode == 0)
                    return Redirect($"/payment/success?bookingId={orderId}");

                var message = queryParams.GetValueOrDefault("message", "Payment failed");
                return Redirect($"/payment/failed?message={Uri.EscapeDataString(message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Momo return");
                return Redirect("/payment/failed");
            }
        }
    }

    public record CreatePaymentRequest(Guid BookingId);
}
