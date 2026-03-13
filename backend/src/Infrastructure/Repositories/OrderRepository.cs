using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using backend.src.Domain.Enums;
using backend.src.Infrastructure.Persistence.Dapper;
using Dapper;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private const string SelectOrdersSql = @"
        select
            o.id as OrderId,
            o.customer_id as CustomerId,
            o.total_amount as TotalAmount,
            o.status as Status,
            o.created_at as CreatedAt,
            c.id as CustomerEntityId,
            c.name as CustomerName,
            c.email as CustomerEmail,
            c.document as CustomerDocument,
            c.created_at as CustomerCreatedAt,
            oi.id as OrderItemId,
            oi.order_id as OrderItemOrderId,
            oi.product_id as ProductId,
            oi.unit_price as UnitPrice,
            oi.quantity as Quantity,
            oi.line_total as LineTotal,
            p.id as ProductEntityId,
            p.name as ProductName,
            p.sku as ProductSku,
            p.price as ProductPrice,
            p.stock_qty as ProductStockQty,
            p.is_active as ProductIsActive,
            p.created_at as ProductCreatedAt
        from orders o
        inner join customers c on c.id = o.customer_id
        left join order_items oi on oi.order_id = o.id
        left join products p on p.id = oi.product_id";

    private const string SelectTrackedOrderByIdSql = @"
        select
            id as Id,
            customer_id as CustomerId,
            total_amount as TotalAmount,
            status as Status,
            created_at as CreatedAt
        from orders
        where id = @Id
        limit 1;";

    private const string SelectOrderItemsByOrderIdSql = @"
        select
            id as Id,
            order_id as OrderId,
            product_id as ProductId,
            unit_price as UnitPrice,
            quantity as Quantity,
            line_total as LineTotal
        from order_items
        where order_id = @OrderId;";

    private const string DeleteOrderItemsByOrderIdSql = @"
        delete from order_items
        where order_id = @OrderId;";

    private const string DeleteOrdersByCustomerIdSql = @"
        delete from order_items
        where order_id in (
            select id
            from orders
            where customer_id = @CustomerId);

        delete from orders
        where customer_id = @CustomerId;";

    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.GetConnection().QueryAsync<OrderReadRow>(
            _context.CreateCommand($"{SelectOrdersSql} order by o.created_at desc, oi.id;", cancellationToken: cancellationToken));

        return MapOrders(rows);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rows = await _context.GetConnection().QueryAsync<OrderReadRow>(
            _context.CreateCommand($"{SelectOrdersSql} where o.id = @Id order by oi.id;", new { Id = id }, cancellationToken));

        return MapOrders(rows).SingleOrDefault();
    }

    public async Task<Order?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var trackedOrder = _context.Orders.Local.FirstOrDefault(order => order.Id == id);
        if (trackedOrder is not null)
        {
            return trackedOrder;
        }

        var row = await _context.GetConnection().QuerySingleOrDefaultAsync<TrackedOrderRow>(
            _context.CreateCommand(SelectTrackedOrderByIdSql, new { Id = id }, cancellationToken));

        if (row is null)
        {
            return null;
        }

        var order = EntityMaterializer.CreateOrder(
            row.Id,
            row.CustomerId,
            row.TotalAmount,
            ParseStatus(row.Status),
            row.CreatedAt);

        _context.Attach(order);
        return order;
    }

    public async Task<IReadOnlyCollection<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.GetConnection().QueryAsync<OrderItemRow>(
            _context.CreateCommand(SelectOrderItemsByOrderIdSql, new { OrderId = orderId }, cancellationToken));

        return rows.Select(MapOrderItem).ToList();
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public void AddItems(IEnumerable<OrderItem> orderItems)
    {
        _context.OrderItems.AddRange(orderItems);
    }

    public async Task RemoveItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await _context.GetConnection().ExecuteAsync(
            _context.CreateCommand(DeleteOrderItemsByOrderIdSql, new { OrderId = orderId }, cancellationToken));
    }

    public async Task RemoveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        await _context.GetConnection().ExecuteAsync(
            _context.CreateCommand(DeleteOrdersByCustomerIdSql, new { CustomerId = customerId }, cancellationToken));
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyCollection<Order> MapOrders(IEnumerable<OrderReadRow> rows)
    {
        var orders = new Dictionary<Guid, Order>();

        foreach (var row in rows)
        {
            if (!orders.TryGetValue(row.OrderId, out var order))
            {
                var customer = EntityMaterializer.CreateCustomer(
                    row.CustomerEntityId,
                    row.CustomerName,
                    row.CustomerEmail,
                    row.CustomerDocument,
                    row.CustomerCreatedAt);

                order = EntityMaterializer.CreateOrder(
                    row.OrderId,
                    row.CustomerId,
                    row.TotalAmount,
                    ParseStatus(row.Status),
                    row.CreatedAt,
                    customer);

                customer.Orders.Add(order);
                orders[order.Id] = order;
            }

            if (!row.OrderItemId.HasValue || !row.ProductEntityId.HasValue || !row.ProductId.HasValue || !row.UnitPrice.HasValue || !row.Quantity.HasValue || !row.LineTotal.HasValue)
            {
                continue;
            }

            var product = EntityMaterializer.CreateProduct(
                row.ProductEntityId.Value,
                row.ProductName ?? string.Empty,
                row.ProductSku ?? string.Empty,
                row.ProductPrice ?? 0,
                row.ProductStockQty ?? 0,
                row.ProductIsActive ?? false,
                row.ProductCreatedAt ?? default);

            var orderItem = EntityMaterializer.CreateOrderItem(
                row.OrderItemId.Value,
                row.OrderItemOrderId ?? row.OrderId,
                row.ProductId.Value,
                row.UnitPrice.Value,
                row.Quantity.Value,
                row.LineTotal.Value,
                order,
                product);

            order.OrderItems.Add(orderItem);
            product.OrderItems.Add(orderItem);
        }

        return orders.Values.ToList();
    }

    private static OrderItem MapOrderItem(OrderItemRow row)
    {
        return EntityMaterializer.CreateOrderItem(
            row.Id,
            row.OrderId,
            row.ProductId,
            row.UnitPrice,
            row.Quantity,
            row.LineTotal);
    }

    private static OrderStatus ParseStatus(string status)
    {
        return Enum.Parse<OrderStatus>(status, ignoreCase: true);
    }

    private sealed class OrderReadRow
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CustomerEntityId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerDocument { get; set; } = string.Empty;
        public DateTime CustomerCreatedAt { get; set; }
        public Guid? OrderItemId { get; set; }
        public Guid? OrderItemOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? LineTotal { get; set; }
        public Guid? ProductEntityId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSku { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? ProductStockQty { get; set; }
        public bool? ProductIsActive { get; set; }
        public DateTime? ProductCreatedAt { get; set; }
    }

    private sealed class TrackedOrderRow
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    private sealed class OrderItemRow
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
