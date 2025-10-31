using Mapster;
using User.Application.DTOs;
using User.Domain.Entities;

namespace User.Application.Mappings
{
    public class HostProfileMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserEntity, HostProfileDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => src.FullName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
                .Map(dest => dest.Bio, src => src.HostProfile != null ? src.HostProfile.Bio : null)
                .Map(dest => dest.SpokenLanguages, src => src.HostProfile != null ? src.HostProfile.SpokenLanguages : null)
                .Map(dest => dest.Location, src => src.HostProfile != null ? src.HostProfile.Location : null)
                .Map(dest => dest.InstagramUrl, src => src.HostProfile != null ? src.HostProfile.InstagramUrl : null)
                .Map(dest => dest.FacebookUrl, src => src.HostProfile != null ? src.HostProfile.FacebookUrl : null)
                .Map(dest => dest.LinkedInUrl, src => src.HostProfile != null ? src.HostProfile.LinkedInUrl : null)
                .Map(dest => dest.Work, src => src.HostProfile != null ? src.HostProfile.Work : null)
                .Map(dest => dest.Education, src => src.HostProfile != null ? src.HostProfile.Education : null)
                .Map(dest => dest.FunFact, src => src.HostProfile != null ? src.HostProfile.FunFact : null)
                .Map(dest => dest.TopicsOfInterest, src => src.HostProfile != null ? src.HostProfile.TopicsOfInterest : null)
                .Map(dest => dest.RatingAvg, src => src.HostProfile != null ? src.HostProfile.RatingAvg : null)
                .Map(dest => dest.TotalBookings, src => src.HostProfile != null ? src.HostProfile.TotalBookings : 0)
                .Map(dest => dest.TotalReviews, src => src.HostProfile != null ? src.HostProfile.TotalReviews : 0)
                .Map(dest => dest.HostingSince, src => src.HostProfile != null ? src.HostProfile.HostingSince : null)
                .Map(dest => dest.IsVerified, src => src.HostProfile != null ? src.HostProfile.IsVerified : false)
                .Map(dest => dest.ResponseTime, src => src.HostProfile != null ? src.HostProfile.ResponseTime : null);
        }
    }
}