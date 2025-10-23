using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserService.Application.Queries.Admin.ListUsers;

namespace UserService.Api.Swagger
{
    public class RoleIdSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ListUsersQuery))
            {
                if (schema.Properties.ContainsKey("roleName"))
                {
                    var roleNameProperty = schema.Properties["roleName"];
                    
                    // Thêm enum values để Swagger hiển thị dropdown với tên role
                    roleNameProperty.Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("Admin"),
                        new OpenApiString("User"),
                        new OpenApiString("Host")
                    };
                    
                    // Thêm mô tả
                    roleNameProperty.Description = "Lọc theo role: Admin, User, hoặc Host";
                }
            }
        }
    }
}
