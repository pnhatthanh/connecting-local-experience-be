using Booking.Application.Handlers.Commands.CreateBooking;
using Booking.Application.Handlers.Commands.CancelBooking;
using Booking.Application.Handlers.Queries.GetBooking;
using Booking.Application.Handlers.Queries.GetUserBookings;
using Booking.Application.Handlers.Queries.GetHostBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var result = await _mediator.Send(command);
            
            return Ok(new 
            { 
                success = true, 
                paymentUrl = result.Payment?.PaymentUrl,
                message = result.Payment?.PaymentUrl != null 
                    ? "Booking created successfully. Please proceed to payment." 
                    : "Booking created but payment URL not available" 
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var result = await _mediator.Send(new GetBookingQuery(id));
            if (result == null)
                return NotFound(new { success = false, message = "Booking not found" });
            
            return Ok(new { success = true, data = result });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(Guid userId)
        {
            var result = await _mediator.Send(new GetUserBookingsQuery(userId));
            return Ok(new { success = true, data = result });
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetHostBookings(Guid hostId)
        {
            var result = await _mediator.Send(new GetHostBookingsQuery(hostId));
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id, [FromBody] CancelBookingRequest request)
        {
            var command = new CancelBookingCommand(id, request.Reason, request.IsCancelledByHost);
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Booking cancelled successfully" });
        }
    }

    public record CancelBookingRequest(string Reason, bool IsCancelledByHost = false);
}
