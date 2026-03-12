namespace backend.src.Api.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseApiIdempotency(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
