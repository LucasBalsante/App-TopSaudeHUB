using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Application.Products.Dtos;
using backend.src.Application.Products.Services;
using backend.src.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace backend.tests.Unit.Application.Products.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveGerarProximoSkuUsandoPrefixoDoNome()
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
    public async Task CriarAsync_DeveIniciarSequenciaEmUm_QuandoOPrefixoNaoExistir()
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

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarProduto_QuandoProdutoExistir()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);
        var repository = new FakeProductRepository(trackedProduct: product);
        var service = new ProductService(repository, new FakeTransactionManager());
        var request = new UpdateProductRequest
        {
            Name = "Dipirona Gotas",
            Price = 12.5m,
            StockQty = 8,
            IsActive = false
        };

        var result = await service.UpdateAsync(product.Id, request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Dipirona Gotas", product.Name);
        Assert.Equal(12.5m, product.Price);
        Assert.Equal(8, product.StockQty);
        Assert.False(product.IsActive);
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task ExcluirAsync_DeveRetornarConflito_QuandoProdutoPossuirItensRelacionados()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);
        var repository = new FakeProductRepository(trackedProduct: product, hasRelatedOrderItems: true);
        var service = new ProductService(repository, new FakeTransactionManager());

        var result = await service.DeleteAsync(product.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Null(repository.RemovedProduct);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarBadRequest_QuandoNomeDoProdutoForInvalido()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository, new FakeTransactionManager());
        var request = new CreateProductRequest
        {
            Name = "   ",
            Price = 19.9m,
            StockQty = 12,
            IsActive = true
        };

        var result = await service.CreateAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Null(repository.AddedProduct);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task ExcluirAsync_DeveRemoverProduto_QuandoNaoPossuirItensRelacionados()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);
        var repository = new FakeProductRepository(trackedProduct: product);
        var service = new ProductService(repository, new FakeTransactionManager());

        var result = await service.DeleteAsync(product.Id);

        Assert.True(result.IsSuccess);
        Assert.Same(product, repository.RemovedProduct);
        Assert.Equal(1, repository.SaveChangesCalls);
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
        private readonly Product? _product;
        private readonly bool _hasRelatedOrderItems;

        public FakeProductRepository(string? lastSkuByPrefix = null, Product? product = null, Product? trackedProduct = null, bool hasRelatedOrderItems = false)
        {
            _lastSkuByPrefix = lastSkuByPrefix;
            _product = trackedProduct ?? product;
            _hasRelatedOrderItems = hasRelatedOrderItems;
        }

        public Product? AddedProduct { get; private set; }
        public Product? RemovedProduct { get; private set; }
        public int SaveChangesCalls { get; private set; }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);

        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);

        public Task<IReadOnlyCollection<Product>> GetTrackedByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());

        public Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreProductId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<string?> GetLastSkuByPrefixAsync(string skuPrefix, CancellationToken cancellationToken = default)
            => Task.FromResult(_lastSkuByPrefix);

        public Task<bool> HasRelatedOrderItemsAsync(Guid productId, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == productId && _hasRelatedOrderItems);

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            AddedProduct = product;
            return Task.CompletedTask;
        }

        public void Remove(Product product)
        {
            RemovedProduct = product;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }
    }
}
