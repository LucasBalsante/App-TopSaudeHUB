namespace backend.src.Application.Customers.Dtos;

public sealed class CreateCustomerRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Document { get; init; } = string.Empty;
}
