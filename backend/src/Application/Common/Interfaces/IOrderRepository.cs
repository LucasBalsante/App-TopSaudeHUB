using backend.src.Domain.Entities;

namespace backend.src.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void RemoveItems(IEnumerable<OrderItem> orderItems);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
