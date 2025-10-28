using Notification.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNotificationServices(builder.Configuration);
builder.Services.AddNotificationRabbitMQ(builder.Configuration);

var app = builder.Build();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");
await app.SubscribeToEvents();
app.Logger.LogInformation("Notification Service is running...");
app.Run();

