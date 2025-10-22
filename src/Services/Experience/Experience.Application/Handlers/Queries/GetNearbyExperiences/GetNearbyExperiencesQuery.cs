using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetNearbyExperiences
{
    public class GetNearbyExperiencesQuery : IQuery<IEnumerable<ExperienceDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInKm { get; set; } = 10;
    }
}
