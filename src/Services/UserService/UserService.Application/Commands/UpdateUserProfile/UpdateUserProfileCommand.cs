using BuildingBlocks.Application.CQRS.Command;
using UserService.Application.DTOs;

namespace UserService.Application.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : ICommand<UserProfileResponse>
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
