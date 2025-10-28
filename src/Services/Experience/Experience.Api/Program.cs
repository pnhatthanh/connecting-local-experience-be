using Experience.Api.Extensions;
using Experience.Application.Extensions;
using Experience.Infrastructure.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthenticationExtension(builder.Configuration);

builder.Services.AddExperienceApplication()
                .AddExperienceInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.ApplyMigrationAsync();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
