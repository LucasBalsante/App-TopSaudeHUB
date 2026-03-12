using backend.src.Application.Common.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Persistence.Seed
{
    public class DbSeeder : IDbSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Applying database migrations...");
            await _context.Database.MigrateAsync(cancellationToken);

            var hasProducts = await _context.Products.CountAsync(cancellationToken);
            var hasCustomers = await _context.Customers.AnyAsync(cancellationToken);

            if (hasProducts> 0 && hasCustomers)
            {
                _logger.LogInformation("Database already seeded. Skipping seed process.");
                return;
            }

            if (hasProducts >0)
            {
                var products = ProductSeed.GetProducts();
                await _context.Products.AddRangeAsync(products, cancellationToken);
                _logger.LogInformation("Seeding products...");
            }

            if (!hasCustomers)
            {
                var customers = CustomerSeed.GetCustomers();
                await _context.Customers.AddRangeAsync(customers, cancellationToken);
                _logger.LogInformation("Seeding customers...");
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Database seed completed successfully.");
        }
    }
}
