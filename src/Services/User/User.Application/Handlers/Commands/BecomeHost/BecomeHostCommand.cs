using BuildingBlocks.Application.CQRS.Command;
using Microsoft.AspNetCore.Http;
using User.Application.DTOs;

namespace User.Application.Handlers.Commands.BecomeHost
{
    public class BecomeHostCommand : ICommand<HostProfileDto>
    {
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string[]? SpokenLanguages { get; set; }
        public string? Location { get; set; }
        public IFormFile? Document { get; set; }
    }
}
