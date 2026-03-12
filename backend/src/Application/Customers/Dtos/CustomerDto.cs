namespace backend.src.Application.Customers.Dtos;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string Document,
    DateTime CreatedAt);
