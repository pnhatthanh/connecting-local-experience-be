using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Presentation.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            if (permissions.Contains(requirement.PermissionCode))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
