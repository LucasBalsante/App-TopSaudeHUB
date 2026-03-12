namespace backend.src.Application.Orders.Dtos;

public sealed class UpdateOrderItemRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
