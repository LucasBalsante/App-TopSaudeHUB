namespace backend.src.Application.Orders.Dtos;

public sealed class UpdateOrderRequest
{
    public Guid CustomerId { get; init; }
    public IReadOnlyCollection<UpdateOrderItemRequest> Items { get; init; } = Array.Empty<UpdateOrderItemRequest>();
}
