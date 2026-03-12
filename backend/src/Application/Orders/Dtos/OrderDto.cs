using backend.src.Domain.Enums;

namespace backend.src.Application.Orders.Dtos;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyCollection<OrderItemDto> Items);
