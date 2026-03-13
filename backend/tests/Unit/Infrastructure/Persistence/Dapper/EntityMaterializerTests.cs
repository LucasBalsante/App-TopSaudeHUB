using backend.src.Domain.Entities;
using backend.src.Domain.Enums;
using backend.src.Infrastructure.Persistence.Dapper;

namespace backend.tests.Unit.Infrastructure.Persistence.Dapper;

public class EntityMaterializerTests
{
    [Fact]
    public void CreateCustomer_DevePopularPropriedadesEInicializarColecaoDePedidos()
    {
        var order = new Order(Guid.NewGuid(), 25m, OrderStatus.PAID);
        var orders = new[] { order };
        var createdAt = new DateTime(2026, 03, 11, 10, 30, 0, DateTimeKind.Utc);
        var customerId = Guid.NewGuid();

        var customer = EntityMaterializer.CreateCustomer(
            customerId,
            "Lucas Silva",
            "lucas@email.com",
            "12345678900",
            createdAt,
            orders);

        Assert.Equal(customerId, customer.Id);
        Assert.Equal("Lucas Silva", customer.Name);
        Assert.Equal("lucas@email.com", customer.Email);
        Assert.Equal("12345678900", customer.Document);
        Assert.Equal(createdAt, customer.CreatedAt);
        Assert.Single(customer.Orders);
        Assert.Same(order, customer.Orders.Single());
        Assert.NotSame(orders, customer.Orders);
    }

    [Fact]
    public void CreateProduct_DevePopularPropriedadesEInicializarColecaoVazia_QuandoItensNaoForemInformados()
    {
        var productId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 03, 11, 11, 0, 0, DateTimeKind.Utc);

        var product = EntityMaterializer.CreateProduct(
            productId,
            "Dipirona",
            "DIP-0001",
            19.9m,
            15,
            true,
            createdAt);

        Assert.Equal(productId, product.Id);
        Assert.Equal("Dipirona", product.Name);
        Assert.Equal("DIP-0001", product.Sku);
        Assert.Equal(19.9m, product.Price);
        Assert.Equal(15, product.StockQty);
        Assert.True(product.IsActive);
        Assert.Equal(createdAt, product.CreatedAt);
        Assert.Empty(product.OrderItems);
    }

    [Fact]
    public void CreateOrder_DevePopularPropriedadesENavegacoes_QuandoInformadas()
    {
        var customer = new Customer("Lucas Silva", "lucas@email.com", "12345678900");
        var product = new Product("Dipirona", "DIP-0001", 19.9m, 10);
        var orderItem = new OrderItem(Guid.NewGuid(), product.Id, product.Price, 2);
        var orderItems = new[] { orderItem };
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 03, 11, 12, 0, 0, DateTimeKind.Utc);

        var order = EntityMaterializer.CreateOrder(
            orderId,
            customerId,
            39.8m,
            OrderStatus.PAID,
            createdAt,
            customer,
            orderItems);

        Assert.Equal(orderId, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(39.8m, order.TotalAmount);
        Assert.Equal(OrderStatus.PAID, order.Status);
        Assert.Equal(createdAt, order.CreatedAt);
        Assert.Same(customer, order.Customer);
        Assert.Single(order.OrderItems);
        Assert.Same(orderItem, order.OrderItems.Single());
        Assert.NotSame(orderItems, order.OrderItems);
    }

    [Fact]
    public void CreateOrderItem_DevePopularPropriedadesENavegacoes_QuandoInformadas()
    {
        var customer = new Customer("Lucas Silva", "lucas@email.com", "12345678900");
        var order = new Order(customer.Id, 19.9m, OrderStatus.CREATED);
        var product = new Product("Dipirona", "DIP-0001", 19.9m, 10);
        var orderItemId = Guid.NewGuid();

        var orderItem = EntityMaterializer.CreateOrderItem(
            orderItemId,
            order.Id,
            product.Id,
            19.9m,
            2,
            39.8m,
            order,
            product);

        Assert.Equal(orderItemId, orderItem.Id);
        Assert.Equal(order.Id, orderItem.OrderId);
        Assert.Equal(product.Id, orderItem.ProductId);
        Assert.Equal(19.9m, orderItem.UnitPrice);
        Assert.Equal(2, orderItem.Quantity);
        Assert.Equal(39.8m, orderItem.LineTotal);
        Assert.Same(order, orderItem.Order);
        Assert.Same(product, orderItem.Product);
    }
}
