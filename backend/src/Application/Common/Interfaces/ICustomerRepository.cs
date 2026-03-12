using backend.src.Domain.Entities;

namespace backend.src.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid? ignoreCustomerId = null, CancellationToken cancellationToken = default);
    Task<bool> HasRelatedOrdersAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    void Remove(Customer customer);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
