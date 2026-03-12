namespace backend.src.Application.Orders.Dtos;

public sealed class CreateOrderRequest
{
    public Guid CustomerId { get; init; }
    public IReadOnlyCollection<CreateOrderItemRequest> Items { get; init; } = Array.Empty<CreateOrderItemRequest>();
}
