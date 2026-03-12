namespace backend.src.Application.Orders.Dtos;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
