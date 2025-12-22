using BuildingBlocks.Application.CQRS.Command;
using Microsoft.AspNetCore.Http;
using User.Application.DTOs;

namespace User.Application.Handlers.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : ICommand<UserDto>
    {
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Country { get; set; }
    }
}
