using backend.src.Application.Common.Interfaces;
using backend.src.Api.Endpoints;
using backend.src.Api.Middlewares;

namespace backend.src.Api.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseApiIdempotency();
        app.UseApiExceptionHandling();

        return app;
    }

    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
        await seeder.SeedAsync();

        return app;
    }

    public static WebApplication MapApiRoutes(this WebApplication app)
    {
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

        return app;
    }
}
