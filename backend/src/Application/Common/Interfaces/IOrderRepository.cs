using backend.src.Domain.Entities;

namespace backend.src.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void AddItems(IEnumerable<OrderItem> orderItems);
    Task RemoveItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task RemoveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
