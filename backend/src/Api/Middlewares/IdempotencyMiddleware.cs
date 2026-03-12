using System.Text;
using backend.src.Application.Common.Interfaces;
using backend.src.Infrastructure.Services;

namespace backend.src.Api.Middlewares;

public sealed class IdempotencyMiddleware
{
    private const string IdempotencyHeaderName = "Idempotency-Key";

    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers[IdempotencyHeaderName].ToString().Trim();

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await WriteErrorAsync(context, "O header Idempotency-Key é obrigatório para operações POST.", StatusCodes.Status400BadRequest);
            return;
        }

        context.Request.EnableBuffering();

        string requestBody;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var requestPath = context.Request.Path.Value ?? string.Empty;
        var requestHash = PayloadHasher.ComputeSha256(requestBody);
        var checkResult = await idempotencyService.ReserveAsync(idempotencyKey, requestPath, requestHash, context.RequestAborted);

        if (checkResult.IsPayloadMismatch)
        {
            await WriteErrorAsync(context, "A Idempotency-Key informada já foi utilizada com um payload diferente.", StatusCodes.Status409Conflict);
            return;
        }

        if (checkResult.IsProcessing)
        {
            await WriteErrorAsync(context, "Já existe uma requisição em processamento para a Idempotency-Key informada.", StatusCodes.Status409Conflict);
            return;
        }

        if (checkResult.ShouldReturnStoredResponse)
        {
            context.Response.StatusCode = checkResult.StoredResponseCode ?? StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(checkResult.StoredResponsePayload ?? string.Empty);
            return;
        }

        var originalBodyStream = context.Response.Body;
        await using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);

            responseBodyStream.Position = 0;
            var responsePayload = await new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            responseBodyStream.Position = 0;

            if (context.Response.StatusCode is >= 200 and < 300)
            {
                await idempotencyService.CompleteAsync(
                    checkResult.IdempotencyRequestId!.Value,
                    context.Response.StatusCode,
                    responsePayload,
                    null,
                    context.RequestAborted);
            }
            else
            {
                await idempotencyService.FailAsync(
                    checkResult.IdempotencyRequestId!.Value,
                    context.Response.StatusCode,
                    responsePayload,
                    context.RequestAborted);
            }

            await responseBodyStream.CopyToAsync(originalBodyStream, context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, string message, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Error(message));
    }
}
