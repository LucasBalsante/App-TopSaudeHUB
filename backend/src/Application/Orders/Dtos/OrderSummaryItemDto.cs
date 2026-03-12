namespace backend.src.Application.Orders.Dtos;

public sealed record OrderSummaryItemDto(
    OrderProductSummaryDto Product,
    int Quantidade,
    decimal LineTotal);
