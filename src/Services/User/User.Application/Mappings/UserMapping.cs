using Mapster;
using User.Application.DTOs;
using User.Domain.Entities;

namespace User.Application.Mappings
{
    public class UserMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserEntity, UserDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => src.FullName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
                .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
                .Map(dest => dest.Country, src => src.Country)
                .Map(dest => dest.Gender, src => src.Gender.ToString())
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.Role, src => src.Role.ToString())
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.HostProfile, src => src.HostProfile ?? null);
            
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
}