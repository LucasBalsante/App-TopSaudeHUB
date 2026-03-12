using backend.src.Domain.Entities;

namespace backend.tests.Unit.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldNormalizeProperties_WhenDataIsValid()
    {
        var customer = new Customer("  Lucas Silva  ", "  LUCAS@EMAIL.COM  ", " 12345678900 ");

        Assert.Equal("Lucas Silva", customer.Name);
        Assert.Equal("lucas@email.com", customer.Email);
        Assert.Equal("12345678900", customer.Document);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenEmailIsInvalid()
    {
        var action = () => new Customer("Lucas Silva", "email-invalido", "12345678900");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("email", exception.ParamName);
        Assert.Contains("inválido", exception.Message);
    }
}
