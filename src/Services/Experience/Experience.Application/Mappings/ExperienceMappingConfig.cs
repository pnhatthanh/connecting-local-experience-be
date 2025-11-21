using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Mapster;

namespace Experience.Application.Mappings
{
    public class ExperienceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ExperienceEntity, ExperienceDto>()
                .Map(dest => dest.Category, src => src.Category != null ? new CategoryDto
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name
                } : null)
                .Map(dest => dest.ActivityLevel, src => src.ActivityLevel.ToString())
                .Map(dest => dest.SkillLevel, src => src.SkillLevel.ToString())
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.CancellationPolicy, src => src.CancellationPolicy.ToString())
                .Map(dest => dest.Media, src => src.Media)
                .Map(dest => dest.Itineraries, src => src.Itineraries);
            config.NewConfig<ExperienceMediaEntity, ExperienceMediaDto>();
            config.NewConfig<ExperienceItineraryEntity, ExperienceItineraryDto>();
            config.NewConfig<ExperienceEntity, ExperienceSummaryDto>()
                .Map(dest => dest.Category, src => src.Category != null ? new CategoryDto
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name
                } : null)
                .Map(dest => dest.Media, src => src.Media)
                .Map(dest => dest.AverageRating, src => src.AverageRating)
                .Map(dest => dest.TotalReviews, src => src.TotalReviews)
                .Map(dest => dest.Address, src => src.Address + ", " + src.District + ", " + src.City + ", " + src.Country);
        }
    }
}
