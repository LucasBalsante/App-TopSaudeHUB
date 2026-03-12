using backend.src.Application.Common.Models;
using backend.src.Application.Products.Dtos;
using backend.src.Application.Products.Interfaces;
using backend.src.Infrastructure.Services;

namespace backend.src.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (IProductService productService, CancellationToken cancellationToken) =>
        {
            var products = await productService.GetAllAsync(cancellationToken);
            return Results.Ok(ApiResponse<IReadOnlyCollection<ProductDto>>.Success(products, "Produtos listados com sucesso."));
        });

        group.MapGet("/{id:guid}", async (Guid id, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.GetByIdAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<ProductDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        }).WithName("GetProductById");

        group.MapPost("/", async (CreateProductRequest request, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.CreateAsync(request, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/products/{result.Data!.Id}", ApiResponse<ProductDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.UpdateAsync(id, request, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(ApiResponse<ProductDto>.Success(result.Data!, result.Message))
                : ToErrorResult(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.DeleteAsync(id, cancellationToken);
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
