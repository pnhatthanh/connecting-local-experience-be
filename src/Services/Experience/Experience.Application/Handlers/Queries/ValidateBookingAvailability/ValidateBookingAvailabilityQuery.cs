using BuildingBlocks.Application.CQRS.Query;

namespace Experience.Application.Handlers.Queries.ValidateBookingAvailability
{
    public record ValidateBookingAvailabilityQuery(
        Guid ExperienceId,
        DateOnly Date,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int Adults,
        int Children
    ) : IQuery<bool>;
}
