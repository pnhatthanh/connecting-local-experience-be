using Mapster;
using User.Application.DTOs;
using User.Domain.Entities;

namespace User.Application.Mappings;

public class HostSummaryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserEntity, HostSummaryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
            .Map(dest => dest.Location, src => src.HostProfile != null ? src.HostProfile.Location : string.Empty)
            .Map(dest => dest.Work, src => src.HostProfile != null ? src.HostProfile.Work : null)
            .Map(dest => dest.Education, src => src.HostProfile != null ? src.HostProfile.Education : null)
            .Map(dest => dest.IsVerified, src => src.HostProfile != null && src.HostProfile.IsVerified)
            .Map(dest => dest.VerifyStatus, src => src.HostProfile != null ? src.HostProfile.VerifyStatus.ToString() : "Unverified");
    }
}