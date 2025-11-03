using Mapster;
using Experience.Application.Dtos;
using Experience.Domain.Entities;

namespace Experience.Application.Mappings
{
    public class CategoryMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ExperienceCategoryEntity, CategoryDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name);
        }
    }
}
