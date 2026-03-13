using backend.src.Domain.Entities;
using backend.src.Domain.Enums;

namespace backend.tests.Unit.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Construtor_DeveIniciarComStatusCriado_EValorTotalZero()
    {
        var order = new Order(Guid.NewGuid());

        Assert.Equal(OrderStatus.CREATED, order.Status);
        Assert.Equal(0m, order.TotalAmount);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarItemAoPedido_EAumentarValorTotal()
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

    [Fact]
    public void LimparItens_DeveRemoverItensERedefinirValorTotal()
    {
        var order = new Order(Guid.NewGuid());
        var product = new Product("Dipirona", "DIP-0001", 12.5m, 10);

        order.AddItem(product, 2);
        order.ClearItems();

        Assert.Empty(order.OrderItems);
        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void AtualizarStatus_DeveAlterarStatus_QuandoValorForValido()
    {
        var order = new Order(Guid.NewGuid());

        order.UpdateStatus(OrderStatus.PAID);

        Assert.Equal(OrderStatus.PAID, order.Status);
    }

    [Fact]
    public void AtualizarStatus_DeveLancarArgumentException_QuandoStatusForInvalido()
    {
        var order = new Order(Guid.NewGuid());

        var action = () => order.UpdateStatus((OrderStatus)999);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("status", exception.ParamName);
        Assert.Contains("inválido", exception.Message);
    }

    [Fact]
    public void AtualizarCliente_DeveLancarArgumentException_QuandoCustomerIdForVazio()
    {
        var order = new Order(Guid.NewGuid());

        var action = () => order.UpdateCustomer(Guid.Empty);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("customerId", exception.ParamName);
        Assert.Contains("obrigatório", exception.Message);
    }
}
