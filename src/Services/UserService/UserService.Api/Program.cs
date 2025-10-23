using BuildingBlocks.EntityFramework;
using BuildingBlocks.Presentation;
using UserService.Api.Swagger;
using UserService.Application.Extensions;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "User Service API", Version = "v1" });
    c.SchemaFilter<RoleIdSchemaFilter>();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Application & Infrastructure layers
builder.Services.AddUserApplication();
builder.Services.AddUserInfrastructure(builder.Configuration);

// Authentication & Authorization (sẽ cần JWT từ IAM service)
// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options =>
//     {
//         // TODO: Configure JWT authentication từ IAM service
//         // options.Authority = "http://localhost:5001";
//         // options.RequireHttpsMetadata = false;
//         // options.Audience = "user-service";
//     });

// builder.Services.AddAuthorization();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Use default middlewares for exception handling and security headers
// app.UseDefaultMiddlewares(); // Temporarily disabled

// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

// Apply migrations on startup
try
{
    await app.Services.ApplyMigrationsAsync<UserDbContext>();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while applying migrations");
}

app.Run();