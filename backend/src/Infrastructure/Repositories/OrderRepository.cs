using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<Order?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .Where(orderItem => orderItem.OrderId == orderId)
            .ToListAsync(cancellationToken);
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
        await _context.OrderItems
            .Where(orderItem => orderItem.OrderId == orderId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task RemoveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orderIds = await _context.Orders
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId)
            .Select(order => order.Id)
            .ToListAsync(cancellationToken);

        if (orderIds.Count == 0)
        {
            return;
        }

        await _context.OrderItems
            .Where(orderItem => orderIds.Contains(orderItem.OrderId))
            .ExecuteDeleteAsync(cancellationToken);

        await _context.Orders
            .Where(order => order.CustomerId == customerId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
