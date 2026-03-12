using backend.src.Application.Common.Models;
using backend.src.Application.Products.Dtos;

namespace backend.src.Application.Products.Interfaces;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OperationResult<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
