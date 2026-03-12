using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetTrackedByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var productIds = ids.Distinct().ToList();

        return await _context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreProductId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();

        return await _context.Products
            .AsNoTracking()
            .AnyAsync(product => product.Sku == normalizedSku && (!ignoreProductId.HasValue || product.Id != ignoreProductId.Value), cancellationToken);
    }

    public async Task<bool> HasRelatedOrderItemsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .AnyAsync(orderItem => orderItem.ProductId == productId, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
