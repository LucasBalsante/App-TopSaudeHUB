using backend.src.Application.Common.Interfaces;
using backend.src.Infrastructure.Persistence.Seed;
using backend.src.Infrastructure.Repositories;
using backend.src.Infrastructure.Services;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace backend.src.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IDbSeeder, DbSeeder>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
