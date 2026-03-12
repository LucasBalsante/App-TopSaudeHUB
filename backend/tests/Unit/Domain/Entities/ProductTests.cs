using backend.src.Domain.Entities;

namespace backend.tests.Unit.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Update_ShouldKeepExistingSku_WhenOtherFieldsChange()
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
    public void DecreaseStock_ShouldThrowArgumentException_WhenProductIsInactive()
    {
        var product = new Product("Dipirona", "DIP-0001", 10m, 5, false);

        var action = () => product.DecreaseStock(1);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("quantity", exception.ParamName);
        Assert.Contains("inativo", exception.Message);
    }
}
