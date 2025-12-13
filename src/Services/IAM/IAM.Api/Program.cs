using IAM.Infrastructure.Extensions;
using IAM.Application.Extensions;
using IAM.Api.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddIAMApplication()
                .AddIAMInfrastructure(builder.Configuration)
                .AddAuthenticationExtension(builder.Configuration)
                .AddPermissionAuthorization();

var app = builder.Build();

await app.Services.ApplyMigrationAsync();
await app.Services.SeedDataAsync();
await app.Services.SubscribeToEventsAsync();

app.UseException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();