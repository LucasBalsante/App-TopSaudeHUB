using backend.src.Application.Common.Interfaces;
using backend.src.Application;
using backend.src.Api.Configuration;
using backend.src.Api.Endpoints;
using backend.src.Api.Middlewares;
using backend.src.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

EnvironmentFileLoader.LoadIfExists(
    Path.Combine(builder.Environment.ContentRootPath, ".env"),
    Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));

const string FrontendCorsPolicy = "FrontendCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowAnyHeader();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("src/Api/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"src/Api/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCors(FrontendCorsPolicy);
app.UseApiIdempotency();
app.UseApiExceptionHandling();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
    await seeder.SeedAsync();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "TopSaudeHub Backend",
    status = "online",
    endpoints = new[]
    {
        "GET /health",
        "GET /api",
        "GET /api/products/",
        "GET /api/products/{id}",
        "POST /api/products/",
        "PUT /api/products/{id}",
        "DELETE /api/products/{id}",
        "GET /api/customers/",
        "GET /api/customers/{id}",
        "POST /api/customers/",
        "PUT /api/customers/{id}",
        "DELETE /api/customers/{id}",
        "GET /api/orders/{id}",
        "POST /api/orders/"
    }
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    environment = app.Environment.EnvironmentName
}));

app.MapGet("/api", () => Results.Ok(new
{
    resources = new[]
    {
        "/api/products/",
        "/api/customers/",
        "/api/orders/"
    }
}));

app.MapProductEndpoints();
app.MapCustomerEndpoints();
app.MapOrderEndpoints();

app.Run();