using backend.src.Domain.Entities;
using backend.src.Domain.Enums;

namespace backend.tests.Unit.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldStartWithCreatedStatus_AndZeroTotalAmount()
    {
        var order = new Order(Guid.NewGuid());

        Assert.Equal(OrderStatus.CREATED, order.Status);
        Assert.Equal(0m, order.TotalAmount);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void AddItem_ShouldAddOrderItem_AndIncreaseTotalAmount()
    {
        var order = new Order(Guid.NewGuid());
        var product = new Product("Dipirona", "DIP-0001", 12.5m, 10);

        order.AddItem(product, 3);

        var item = Assert.Single(order.OrderItems);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(37.5m, item.LineTotal);
        Assert.Equal(37.5m, order.TotalAmount);
    }
}
