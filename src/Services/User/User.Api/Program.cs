using User.Application.Extensions;
using User.Infrastructure.Extensions;
using User.Api.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUserApplication()
                .AddUserInfrastructure(builder.Configuration)
                .AddAuthenticationExtension(builder.Configuration)
                .AddPermissionAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

await app.Services.ApplyMigrationAsync();
await app.Services.SubscribeToEventsAsync();

app.UseException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
