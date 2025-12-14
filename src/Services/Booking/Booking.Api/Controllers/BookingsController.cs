using Booking.Application.Handlers.Commands.CreateBooking;
using Booking.Application.Handlers.Commands.CancelBooking;
using Booking.Application.Handlers.Commands.ToggleBookingStatus;
using Booking.Application.Handlers.Queries.CheckCompletedBooking;
using Booking.Application.Handlers.Queries.GetBooking;
using Booking.Application.Handlers.Queries.GetUserBookings;
using Booking.Application.Handlers.Queries.GetHostBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Presentation.Authorization;
using BuildingBlocks.Presentation.Constants;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.BOOKING_BOOKING_CREATE)]
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

        [HttpGet("host-bookings")]
        [RequirePermission(PermissionCodes.BOOKING_BOOKING_VIEW_BY_HOST)]
        public async Task<IActionResult> GetHostBookings([FromQuery] GetHostBookingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{id}/cancel")]
        [RequirePermission(PermissionCodes.BOOKING_BOOKING_CANCEL)]
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

        [HttpPatch("{id}/status")]
        [RequirePermission(PermissionCodes.BOOKING_BOOKING_TOGGLE_STATUS)]
        public async Task<IActionResult> ToggleBookingStatus([FromRoute] Guid id, [FromBody] ToggleBookingStatusCommand command)
        {
            var result = await _mediator.Send(command with { BookingId = id });
            return Ok(new { success = result, message = "Booking status toggled successfully" });
        }
    }
}
