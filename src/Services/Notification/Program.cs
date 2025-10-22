using Notification.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Notification Services
builder.Services.AddNotificationServices(builder.Configuration);

// Add RabbitMQ
builder.Services.AddNotificationRabbitMQ(builder.Configuration);

var app = builder.Build();

// Configure HTTP pipeline
app.UseHttpsRedirection();

// Health checks
app.MapHealthChecks("/health");

// Subscribe to RabbitMQ events
await app.SubscribeToEvents();

app.Logger.LogInformation("Notification Service is running...");

app.Run();

