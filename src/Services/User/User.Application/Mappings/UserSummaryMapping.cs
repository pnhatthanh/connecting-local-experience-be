using Mapster;
using User.Application.DTOs;
using User.Domain.Entities;

namespace User.Application.Mappings;

public class UserSummaryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserEntity, UserSummaryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.Role, src => src.Role.ToString())
            .Map(dest => dest.JoinedDate, src => DateOnly.FromDateTime(src.CreatedAt))
            .Map(dest => dest.IsActive, src => src.Status == UserStatus.Active);
    }
}