using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Presentation.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "Permission:";
        public string PermissionCode { get; }
        
        public RequirePermissionAttribute(string permissionCode)
        {
            Policy = $"{PolicyPrefix}{permissionCode}";
            PermissionCode = permissionCode;
        }
    }
}
