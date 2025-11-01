using BuildingBlocks.Application.CQRS.Command;
using Microsoft.AspNetCore.Http;
using User.Application.DTOs;

namespace User.Application.Handlers.Commands.BecomeHost
{
    public class BecomeHostCommand : ICommand<HostProfileDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public IFormFile Avatar { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string[] SpokenLanguages { get; set; } = [];
        public string Location { get; set; } = string.Empty;
        public IFormFile Document { get; set; } = null!;
        public string Work { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string FunFact { get; set; } = string.Empty;
        public string[] TopicsOfInterest { get; set; } = [];
        public string? DesiredHostingStyle { get; set; }
        public string? ResponseTime { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }
}
