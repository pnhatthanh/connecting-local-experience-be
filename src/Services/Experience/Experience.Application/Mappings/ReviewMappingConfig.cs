using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Mapster;

namespace Experience.Application.Mappings
{
    public class ReviewMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ReviewEntity, ReviewDto>();
        }
    }
}
