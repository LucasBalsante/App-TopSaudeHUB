namespace backend.src.Application.Products.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    int StockQty,
    bool IsActive,
    DateTime CreatedAt);
