using User.Application.Extensions;
using User.Infrastructure.Extensions;
using User.Api.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddUserApplication()
                .AddUserInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
await app.Services.ApplyMigrationAsync();
await app.Services.SubscribeToEventsAsync();

app.UseException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
