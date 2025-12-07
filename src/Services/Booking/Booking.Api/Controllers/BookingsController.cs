using Booking.Application.Handlers.Commands.CreateBooking;
using Booking.Application.Handlers.Commands.CancelBooking;
using Booking.Application.Handlers.Queries.CheckCompletedBooking;
using Booking.Application.Handlers.Queries.GetBooking;
using Booking.Application.Handlers.Queries.GetUserBookings;
using Booking.Application.Handlers.Queries.GetHostBookings;
using Booking.Application.Handlers.Queries.GetExperienceBookings;
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
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";    
            var result = await _mediator.Send(command with { IpAddress = ipAddress });
            return Ok(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserBookings()
        {
            var result = await _mediator.Send(new GetUserBookingsQuery());
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var result = await _mediator.Send(new GetBookingQuery(id));
            
            return Ok(result);
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetHostBookings(Guid hostId)
        {
            var result = await _mediator.Send(new GetHostBookingsQuery(hostId));
            return Ok(new { success = true, data = result });
        }

        [HttpGet("experience/{experienceId}")]
        public async Task<IActionResult> GetExperienceBookings(
            Guid experienceId,
            [FromQuery] DateOnly? date = null,
            [FromQuery] TimeSpan? startTime = null)
        {
            var result = await _mediator.Send(new GetExperienceBookingsQuery(experienceId, date, startTime));
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking([FromRoute] Guid id, [FromBody] CancelBookingCommand command)
        {
            var result = await _mediator.Send(command with { BookingId = id });
            return Ok(new { success = result, message = "Booking cancelled successfully" });
        }

        [HttpGet("check-completed")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckCompletedBooking([FromQuery] Guid userId, [FromQuery] Guid experienceId)
        {
            var result = await _mediator.Send(new CheckCompletedBookingQuery(userId, experienceId));
            return Ok(result);
        }
    }
}
