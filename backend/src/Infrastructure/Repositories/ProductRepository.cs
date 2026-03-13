using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using backend.src.Infrastructure.Persistence.Dapper;
using Dapper;
using Infrastructure.Persistence.Context;

namespace backend.src.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private const string SelectProductsSql = @"
        select
            id as Id,
            name as Name,
            sku as Sku,
            price as Price,
            stock_qty as StockQty,
            is_active as IsActive,
            created_at as CreatedAt
        from products
        order by name;";

    private const string SelectProductByIdSql = @"
        select
            id as Id,
            name as Name,
            sku as Sku,
            price as Price,
            stock_qty as StockQty,
            is_active as IsActive,
            created_at as CreatedAt
        from products
        where id = @Id
        limit 1;";

    private const string SelectProductsByIdsSql = @"
        select
            id as Id,
            name as Name,
            sku as Sku,
            price as Price,
            stock_qty as StockQty,
            is_active as IsActive,
            created_at as CreatedAt
        from products
        where id = any(@Ids);";

    private const string ExistsBySkuSql = @"
        select exists(
            select 1
            from products
            where sku = @Sku
              and (@IgnoreProductId is null or id <> @IgnoreProductId));";

    private const string SelectLastSkuByPrefixSql = @"
        select sku
        from products
        where sku like @Pattern
        order by sku desc
        limit 1;";

    private const string HasRelatedOrderItemsSql = @"
        select exists(
            select 1
            from order_items
            where product_id = @ProductId);";

    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.GetConnection().QueryAsync<ProductRow>(
            _context.CreateCommand(SelectProductsSql, cancellationToken: cancellationToken));

        return rows.Select(MapProduct).ToList();
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _context.GetConnection().QuerySingleOrDefaultAsync<ProductRow>(
            _context.CreateCommand(SelectProductByIdSql, new { Id = id }, cancellationToken));

        return row is null ? null : MapProduct(row);
    }

    public async Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var trackedProduct = _context.Products.Local.FirstOrDefault(product => product.Id == id);
        if (trackedProduct is not null)
        {
            return trackedProduct;
        }

        var row = await _context.GetConnection().QuerySingleOrDefaultAsync<ProductRow>(
            _context.CreateCommand(SelectProductByIdSql, new { Id = id }, cancellationToken));

        if (row is null)
        {
            return null;
        }

        var product = MapProduct(row);
        _context.Attach(product);
        return product;
    }

    public async Task<IReadOnlyCollection<Product>> GetTrackedByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var productIds = ids.Distinct().ToList();

        if (productIds.Count == 0)
        {
            return Array.Empty<Product>();
        }

        var trackedProducts = _context.Products.Local
            .Where(product => productIds.Contains(product.Id))
            .ToDictionary(product => product.Id);

        var missingIds = productIds
            .Where(productId => !trackedProducts.ContainsKey(productId))
            .ToArray();

        if (missingIds.Length == 0)
        {
            return trackedProducts.Values.ToList();
        }

        var rows = await _context.GetConnection().QueryAsync<ProductRow>(
            _context.CreateCommand(SelectProductsByIdsSql, new { Ids = missingIds }, cancellationToken));

        var fetchedProducts = rows.Select(MapProduct).ToList();
        foreach (var product in fetchedProducts)
        {
            _context.Attach(product);
            trackedProducts[product.Id] = product;
        }

        return trackedProducts.Values.ToList();
    }

    public async Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreProductId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();

        return await _context.GetConnection().ExecuteScalarAsync<bool>(
            _context.CreateCommand(
                ExistsBySkuSql,
                new { Sku = normalizedSku, IgnoreProductId = ignoreProductId },
                cancellationToken));
    }

    public async Task<string?> GetLastSkuByPrefixAsync(string skuPrefix, CancellationToken cancellationToken = default)
    {
        var normalizedPrefix = skuPrefix.Trim().ToUpperInvariant();

        return await _context.GetConnection().QuerySingleOrDefaultAsync<string>(
            _context.CreateCommand(
                SelectLastSkuByPrefixSql,
                new { Pattern = $"{normalizedPrefix}-%" },
                cancellationToken));
    }

    public async Task<bool> HasRelatedOrderItemsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.GetConnection().ExecuteScalarAsync<bool>(
            _context.CreateCommand(HasRelatedOrderItemsSql, new { ProductId = productId }, cancellationToken));
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

    private static Product MapProduct(ProductRow row)
    {
        return EntityMaterializer.CreateProduct(
            row.Id,
            row.Name,
            row.Sku,
            row.Price,
            row.StockQty,
            row.IsActive,
            row.CreatedAt);
    }

    private sealed class ProductRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQty { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
