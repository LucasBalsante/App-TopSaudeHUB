using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? ignoreCustomerId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(customer => customer.Email == normalizedEmail && (!ignoreCustomerId.HasValue || customer.Id != ignoreCustomerId.Value), cancellationToken);
    }

    public async Task<bool> HasRelatedOrdersAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .AnyAsync(order => order.CustomerId == customerId, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public void Remove(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
