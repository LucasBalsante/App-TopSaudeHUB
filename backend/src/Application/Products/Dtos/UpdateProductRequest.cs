namespace backend.src.Application.Products.Dtos;

public sealed class UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQty { get; init; }
    public bool IsActive { get; init; }
}
