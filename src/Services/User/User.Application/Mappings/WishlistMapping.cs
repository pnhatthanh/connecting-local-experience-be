using Mapster;
using User.Application.DTOs;
using User.Domain.Entities;

namespace User.Application.Mappings
{
    public class WishlistMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserWishlistEntity, WishlistDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.ExperienceCount, src => src.ExperienceCount)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
        }
    }
}