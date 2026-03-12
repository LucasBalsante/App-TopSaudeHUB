namespace backend.src.Application.Orders.Dtos;

public sealed record OrderProductSummaryDto(
    Guid Id,
    string Name,
    decimal Price);
