using backend.src.Application.Customers.Interfaces;
using backend.src.Application.Customers.Services;
using backend.src.Application.Orders.Interfaces;
using backend.src.Application.Orders.Services;
using backend.src.Application.Products.Interfaces;
using backend.src.Application.Products.Services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.src.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
