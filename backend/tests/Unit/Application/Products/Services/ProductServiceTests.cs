using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Application.Products.Dtos;
using backend.src.Application.Products.Services;
using backend.src.Domain.Entities;

namespace backend.tests.Unit.Application.Products.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldGenerateNextSkuUsingNamePrefix()
    {
        var repository = new FakeProductRepository("DIP-0007");
        var service = new ProductService(repository, new FakeTransactionManager());
        var request = new CreateProductRequest
        {
            Name = "Dipirona 500mg",
            Price = 19.9m,
            StockQty = 12,
            IsActive = true
        };

        var result = await service.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("DIP-0008", result.Data!.Sku);
        Assert.Equal("DIP-0008", repository.AddedProduct!.Sku);
    }

    [Fact]
    public async Task CreateAsync_ShouldStartSequenceAtOne_WhenPrefixDoesNotExist()
    {
        var repository = new FakeProductRepository(lastSkuByPrefix: null);
        var service = new ProductService(repository, new FakeTransactionManager());
        var request = new CreateProductRequest
        {
            Name = "Aspirina",
            Price = 10m,
            StockQty = 4,
            IsActive = true
        };

        var result = await service.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("ASP-0001", result.Data!.Sku);
        Assert.Equal("ASP-0001", repository.AddedProduct!.Sku);
    }

    private sealed class FakeTransactionManager : ITransactionManager
    {
        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
            => operation(cancellationToken);

        public Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
            => operation(cancellationToken);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly string? _lastSkuByPrefix;

        public FakeProductRepository(string? lastSkuByPrefix)
        {
            _lastSkuByPrefix = lastSkuByPrefix;
        }

        public Product? AddedProduct { get; private set; }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Product?>(null);

        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Product?>(null);

        public Task<IReadOnlyCollection<Product>> GetTrackedByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());

        public Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreProductId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<string?> GetLastSkuByPrefixAsync(string skuPrefix, CancellationToken cancellationToken = default)
            => Task.FromResult(_lastSkuByPrefix);

        public Task<bool> HasRelatedOrderItemsAsync(Guid productId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            AddedProduct = product;
            return Task.CompletedTask;
        }

        public void Remove(Product product)
        {
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
