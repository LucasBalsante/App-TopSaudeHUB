using backend.src.Domain.Entities;

namespace backend.tests.Unit.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Construtor_DeveNormalizarPropriedades_QuandoOsDadosForemValidos()
    {
        var customer = new Customer("  Lucas Silva  ", "  LUCAS@EMAIL.COM  ", " 12345678900 ");

        Assert.Equal("Lucas Silva", customer.Name);
        Assert.Equal("lucas@email.com", customer.Email);
        Assert.Equal("12345678900", customer.Document);
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoOEmailForInvalido()
    {
        var action = () => new Customer("Lucas Silva", "email-invalido", "12345678900");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("email", exception.ParamName);
        Assert.Contains("inválido", exception.Message);
    }

    [Fact]
    public void Atualizar_DeveNormalizarPropriedades_QuandoOsDadosForemValidos()
    {
        var customer = new Customer("Lucas Silva", "lucas@email.com", "12345678900");

        customer.Update("  Ana Souza  ", "  ANA@EMAIL.COM  ", " 998887770001 " );

        Assert.Equal("Ana Souza", customer.Name);
        Assert.Equal("ana@email.com", customer.Email);
        Assert.Equal("998887770001", customer.Document);
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoONomeForVazio()
    {
        var action = () => new Customer("   ", "lucas@email.com", "12345678900");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("obrigatório", exception.Message);
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoODocumentoExcederOLimiteMaximo()
    {
        var document = new string('1', 21);

        var action = () => new Customer("Lucas Silva", "lucas@email.com", document);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("document", exception.ParamName);
        Assert.Contains("no máximo 20 caracteres", exception.Message);
    }

    [Fact]
    public void Atualizar_DeveLancarArgumentException_QuandoOEmailForObrigatorio()
    {
        var customer = new Customer("Lucas Silva", "lucas@email.com", "12345678900");

        var action = () => customer.Update("Ana Souza", "   ", "998887770001");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("email", exception.ParamName);
        Assert.Contains("obrigatório", exception.Message);
    }
}
