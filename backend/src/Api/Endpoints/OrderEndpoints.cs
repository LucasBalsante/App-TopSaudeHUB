using backend.src.Application.Common.Models;
using backend.src.Application.Orders.Dtos;
using backend.src.Application.Orders.Interfaces;
using backend.src.Infrastructure.Services;

namespace backend.src.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/{id:guid}", async (Guid id, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var result = await orderService.GetByIdAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<OrderDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        }).WithName("GetOrderById");

        group.MapPost("/", async (CreateOrderRequest request, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var result = await orderService.CreateAsync(request, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/orders/{result.Data!.Id}", ApiResponse<OrderDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        });

        return app;
    }

    private static IResult ToErrorResult<T>(OperationResult<T> result)
    {
        return Results.Json(ApiResponse<T>.Error(result.Message), statusCode: result.StatusCode);
    }
}
