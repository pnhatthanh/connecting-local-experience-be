namespace BuildingBlocks.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string? Email { get; }
        string[]? Roles { get; }
        bool IsAuthenticated { get; }
    }
}
