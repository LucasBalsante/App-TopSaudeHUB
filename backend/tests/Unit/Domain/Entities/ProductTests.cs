using backend.src.Domain.Entities;

namespace backend.tests.Unit.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Atualizar_DeveManterSkuExistente_QuandoOutrosCamposMudarem()
    {
        var product = new Product("Dipirona", "dip-0001", 10m, 5);

        product.Update("Dipirona Gotas", 12.5m, 8, false);

        Assert.Equal("DIP-0001", product.Sku);
        Assert.Equal("Dipirona Gotas", product.Name);
        Assert.Equal(12.5m, product.Price);
        Assert.Equal(8, product.StockQty);
        Assert.False(product.IsActive);
    }

    [Fact]
    public void BaixarEstoque_DeveLancarArgumentException_QuandoOProdutoEstiverInativo()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5, false);

        var action = () => product.DecreaseStock(1);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("quantity", exception.ParamName);
        Assert.Contains("inativo", exception.Message);
    }

    [Fact]
    public void Construtor_DeveNormalizarNomeESku_QuandoOsDadosForemValidos()
    {
        var product = new Product("  Dipirona Gotas  ", " dip-0001 ", 10m, 5);

        Assert.Equal("Dipirona Gotas", product.Name);
        Assert.Equal("DIP-0001", product.Sku);
    }

    [Fact]
    public void AumentarEstoque_DeveSomarQuantidadeAoEstoqueAtual()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);

        product.IncreaseStock(3);

        Assert.Equal(8, product.StockQty);
    }

    [Fact]
    public void BaixarEstoque_DeveSubtrairQuantidade_QuandoHouverEstoqueSuficiente()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);

        product.DecreaseStock(2);

        Assert.Equal(3, product.StockQty);
    }

    [Fact]
    public void AumentarEstoque_DeveLancarArgumentException_QuandoQuantidadeForMenorOuIgualAZero()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5);

        var action = () => product.IncreaseStock(0);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("quantity", exception.ParamName);
        Assert.Contains("maior que zero", exception.Message);
    }
}
