using System.Collections.Concurrent;
using System.Reflection;
using backend.src.Domain.Entities;
using backend.src.Domain.Enums;

namespace backend.src.Infrastructure.Persistence.Dapper;

internal static class EntityMaterializer
{
    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), Action<object, object?>> SetterCache = new();

    public static Customer CreateCustomer(
        Guid id,
        string name,
        string email,
        string document,
        DateTime createdAt,
        IReadOnlyCollection<Order>? orders = null)
    {
        var customer = CreateInstance<Customer>();
        SetProperty(customer, nameof(Customer.Id), id);
        SetProperty(customer, nameof(Customer.Name), name);
        SetProperty(customer, nameof(Customer.Email), email);
        SetProperty(customer, nameof(Customer.Document), document);
        SetProperty(customer, nameof(Customer.CreatedAt), createdAt);
        SetProperty(customer, nameof(Customer.Orders), orders?.ToList() ?? new List<Order>());
        return customer;
    }

    public static Product CreateProduct(
        Guid id,
        string name,
        string sku,
        decimal price,
        int stockQty,
        bool isActive,
        DateTime createdAt,
        IReadOnlyCollection<OrderItem>? orderItems = null)
    {
        var product = CreateInstance<Product>();
        SetProperty(product, nameof(Product.Id), id);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Sku), sku);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.StockQty), stockQty);
        SetProperty(product, nameof(Product.IsActive), isActive);
        SetProperty(product, nameof(Product.CreatedAt), createdAt);
        SetProperty(product, nameof(Product.OrderItems), orderItems?.ToList() ?? new List<OrderItem>());
        return product;
    }

    public static Order CreateOrder(
        Guid id,
        Guid customerId,
        decimal totalAmount,
        OrderStatus status,
        DateTime createdAt,
        Customer? customer = null,
        IReadOnlyCollection<OrderItem>? orderItems = null)
    {
        var order = CreateInstance<Order>();
        SetProperty(order, nameof(Order.Id), id);
        SetProperty(order, nameof(Order.CustomerId), customerId);
        SetProperty(order, nameof(Order.TotalAmount), totalAmount);
        SetProperty(order, nameof(Order.Status), status);
        SetProperty(order, nameof(Order.CreatedAt), createdAt);
        SetProperty(order, nameof(Order.OrderItems), orderItems?.ToList() ?? new List<OrderItem>());

        if (customer is not null)
        {
            SetProperty(order, nameof(Order.Customer), customer);
        }

        return order;
    }

    public static OrderItem CreateOrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        decimal unitPrice,
        int quantity,
        decimal lineTotal,
        Order? order = null,
        Product? product = null)
    {
        var orderItem = CreateInstance<OrderItem>();
        SetProperty(orderItem, nameof(OrderItem.Id), id);
        SetProperty(orderItem, nameof(OrderItem.OrderId), orderId);
        SetProperty(orderItem, nameof(OrderItem.ProductId), productId);
        SetProperty(orderItem, nameof(OrderItem.UnitPrice), unitPrice);
        SetProperty(orderItem, nameof(OrderItem.Quantity), quantity);
        SetProperty(orderItem, nameof(OrderItem.LineTotal), lineTotal);

        if (order is not null)
        {
            SetProperty(orderItem, nameof(OrderItem.Order), order);
        }

        if (product is not null)
        {
            SetProperty(orderItem, nameof(OrderItem.Product), product);
        }

        return orderItem;
    }

    private static T CreateInstance<T>() where T : class
    {
        return (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
    }

    private static void SetProperty<T>(T instance, string propertyName, object? value) where T : class
    {
        var setter = SetterCache.GetOrAdd((typeof(T), propertyName), key =>
        {
            var property = key.Type.GetProperty(key.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"A propriedade '{key.PropertyName}' não foi encontrada em '{key.Type.Name}'.");

            var setMethod = property.GetSetMethod(nonPublic: true)
                ?? throw new InvalidOperationException($"A propriedade '{key.PropertyName}' em '{key.Type.Name}' não possui setter.");

            return (target, propertyValue) => setMethod.Invoke(target, new[] { propertyValue });
        });

        setter(instance, value);
    }
}
