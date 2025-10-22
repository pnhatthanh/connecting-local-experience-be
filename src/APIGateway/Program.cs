using APIGateway.Extensions;
using BuildingBlocks.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCustomCors(builder.Configuration)
                .AddCustomHealthChecks();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseException();
app.UseSecurityHeader();
app.UseCors();
app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
