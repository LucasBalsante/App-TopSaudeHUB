using backend.src.Domain.Enums;

namespace backend.src.Application.Orders.Dtos;

public sealed record OrderSummaryDto(
    Guid Id,
    OrderCustomerSummaryDto Customer,
    decimal TotalAmount,
    OrderStatus Status,
    IReadOnlyCollection<OrderSummaryItemDto> Items);
