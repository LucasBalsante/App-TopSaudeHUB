using backend.src.Domain.Entities;

namespace backend.src.Application.Common.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Product>> GetTrackedByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreProductId = null, CancellationToken cancellationToken = default);
    Task<bool> HasRelatedOrderItemsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
