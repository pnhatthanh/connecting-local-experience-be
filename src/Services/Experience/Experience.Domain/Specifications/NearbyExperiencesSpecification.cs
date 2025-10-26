using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Experience.Domain.Specifications
{
    public class NearbyExperiencesSpecification(Point UserLocation, double RadiusInMeters) : Specification<ExperienceEntity>
    {
        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            return e => e.Status == ExperienceStatus.Approved && 
                       e.Location.IsWithinDistance(UserLocation, RadiusInMeters);
        }
    }
}
