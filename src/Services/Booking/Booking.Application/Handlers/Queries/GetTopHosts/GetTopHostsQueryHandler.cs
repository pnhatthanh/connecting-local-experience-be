using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.Domain.Repositories;
using Booking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Handlers.Queries.GetTopHosts;

public class GetTopHostsQueryHandler : IRequestHandler<GetTopHostsQuery, List<TopHostStatisticsDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserService _userService;

    public GetTopHostsQueryHandler(
        IBookingRepository bookingRepository,
        IUserService userService)
    {
        _bookingRepository = bookingRepository;
        _userService = userService;
    }

    public async Task<List<TopHostStatisticsDto>> Handle(GetTopHostsQuery request, CancellationToken cancellationToken)
    {
        var bookings = _bookingRepository.GetQueryable()
            .Where(b => b.CreatedAt.Year == request.Year 
                && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed));

        var hostStats = await bookings
            .GroupBy(b => b.HostId)
            .Select(g => new
            {
                HostId = g.Key,
                TotalBookings = g.Count(),
                TotalRevenue = g.Sum(b => b.TotalPrice)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(request.Limit)
            .ToListAsync();

        var hostIds = hostStats.Select(x => x.HostId).ToList();
        var hostProfiles = await _userService.GetHostProfilesAsync(hostIds, cancellationToken);
        
        var result = hostStats.Select(stat =>
        {
            var hostData = hostProfiles.FirstOrDefault(h => h.Id == stat.HostId);
            return new TopHostStatisticsDto
            {
                HostId = stat.HostId,
                HostName = hostData?.FullName ?? "Unknown",
                ExperienceCount = hostData?.TotalExperiences ?? 0,
                TotalBookings = stat.TotalBookings,
                TotalRevenue = stat.TotalRevenue,
                AverageRating = hostData?.RatingAvg ?? 0
            };
        }).ToList();

        return result;
    }
}
