using APIGateway.Extensions;
using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCustomCors(builder.Configuration)
                .AddCustomHealthChecks();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseForwardedHeaders();
app.UseException();
app.UseSecurityHeader();
app.UseCors();
app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
