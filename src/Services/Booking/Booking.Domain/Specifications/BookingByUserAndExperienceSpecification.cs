using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;

namespace Booking.Domain.Specifications
{
    public class BookingByUserAndExperienceSpecification(Guid userId, Guid experienceId) : Specification<BookingEntity>
    {
        private readonly Guid _userId = userId;
        private readonly Guid _experienceId = experienceId;

        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => booking.UserId == _userId 
                && booking.ExperienceId == _experienceId
                && booking.Status == BookingStatus.Completed;
        }
    }
}
