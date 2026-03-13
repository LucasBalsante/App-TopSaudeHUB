using backend.src.Domain.Entities;

namespace backend.tests.Unit.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void Construtor_DeveCalcularValorTotalDaLinha_QuandoDadosForemValidos()
    {
        var orderItem = new OrderItem(Guid.NewGuid(), Guid.NewGuid(), 12.5m, 3);

        Assert.Equal(12.5m, orderItem.UnitPrice);
        Assert.Equal(3, orderItem.Quantity);
        Assert.Equal(37.5m, orderItem.LineTotal);
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoQuantidadeForMenorOuIgualAZero()
    {
        var action = () => new OrderItem(Guid.NewGuid(), Guid.NewGuid(), 12.5m, 0);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("quantity", exception.ParamName);
        Assert.Contains("maior que zero", exception.Message);
    }
}
