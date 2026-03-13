using backend.src.Application.Common.Interfaces;
using backend.src.Domain.Entities;
using backend.src.Infrastructure.Persistence.Dapper;
using Dapper;
using Infrastructure.Persistence.Context;

namespace backend.src.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private const string SelectCustomersSql = @"
        select
            id as Id,
            name as Name,
            email as Email,
            document as Document,
            created_at as CreatedAt
        from customers
        order by name;";

    private const string SelectCustomerByIdSql = @"
        select
            id as Id,
            name as Name,
            email as Email,
            document as Document,
            created_at as CreatedAt
        from customers
        where id = @Id
        limit 1;";

    private const string ExistsByEmailSql = @"
        select exists(
            select 1
            from customers
            where email = @Email
              and (@IgnoreCustomerId is null or id <> @IgnoreCustomerId));";

    private const string HasRelatedOrdersSql = @"
        select exists(
            select 1
            from orders
            where customer_id = @CustomerId);";

    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _context.GetConnection().QueryAsync<CustomerRow>(
            _context.CreateCommand(SelectCustomersSql, cancellationToken: cancellationToken));

        return rows.Select(MapCustomer).ToList();
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _context.GetConnection().QuerySingleOrDefaultAsync<CustomerRow>(
            _context.CreateCommand(SelectCustomerByIdSql, new { Id = id }, cancellationToken));

        return row is null ? null : MapCustomer(row);
    }

    public async Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var trackedCustomer = _context.Customers.Local.FirstOrDefault(customer => customer.Id == id);
        if (trackedCustomer is not null)
        {
            return trackedCustomer;
        }

        var row = await _context.GetConnection().QuerySingleOrDefaultAsync<CustomerRow>(
            _context.CreateCommand(SelectCustomerByIdSql, new { Id = id }, cancellationToken));

        if (row is null)
        {
            return null;
        }

        var customer = MapCustomer(row);
        _context.Attach(customer);
        return customer;
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? ignoreCustomerId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _context.GetConnection().ExecuteScalarAsync<bool>(
            _context.CreateCommand(
                ExistsByEmailSql,
                new { Email = normalizedEmail, IgnoreCustomerId = ignoreCustomerId },
                cancellationToken));
    }

    public async Task<bool> HasRelatedOrdersAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.GetConnection().ExecuteScalarAsync<bool>(
            _context.CreateCommand(HasRelatedOrdersSql, new { CustomerId = customerId }, cancellationToken));
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

    private static Customer MapCustomer(CustomerRow row)
    {
        return EntityMaterializer.CreateCustomer(row.Id, row.Name, row.Email, row.Document, row.CreatedAt);
    }

    private sealed class CustomerRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
