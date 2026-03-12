using backend.src.Application.Common.Models;
using backend.src.Application.Customers.Dtos;
using backend.src.Application.Customers.Interfaces;
using backend.src.Infrastructure.Services;

namespace backend.src.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapGet("/", async (ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var customers = await customerService.GetAllAsync(cancellationToken);
            return Results.Ok(ApiResponse<IReadOnlyCollection<CustomerDto>>.Success(customers, "Clientes listados com sucesso."));
        });

        group.MapGet("/{id:guid}", async (Guid id, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.GetByIdAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<CustomerDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        }).WithName("GetCustomerById");

        group.MapPost("/", async (CreateCustomerRequest request, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.CreateAsync(request, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/customers/{result.Data!.Id}", ApiResponse<CustomerDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateCustomerRequest request, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.UpdateAsync(id, request, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<CustomerDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<object?>.Success(null, result.Message))
                : ToErrorResult(result);
        });

        return app;
    }

    private static IResult ToErrorResult<T>(OperationResult<T> result)
    {
        return Results.Json(ApiResponse<T>.Error(result.Message), statusCode: result.StatusCode);
    }

    private static IResult ToErrorResult(OperationResult result)
    {
        return Results.Json(ApiResponse<object?>.Error(result.Message), statusCode: result.StatusCode);
    }
}
