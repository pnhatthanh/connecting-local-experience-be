using Booking.Api.Extensions;
using Booking.Application.Extensions;
using Booking.Infrastructure.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthenticationExtension(builder.Configuration);
builder.Services.AddBookingApplication()
                .AddBookingInfrastructure(builder.Configuration)
                .AddPermissionAuthorization();

var app = builder.Build();

await app.Services.ApplyMigrationAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseException();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

