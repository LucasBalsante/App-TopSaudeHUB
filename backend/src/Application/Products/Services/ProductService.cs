using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Application.Products.Dtos;
using backend.src.Application.Products.Interfaces;
using backend.src.Domain.Entities;

namespace backend.src.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ITransactionManager _transactionManager;

    public ProductService(IProductRepository productRepository, ITransactionManager transactionManager)
    {
        _productRepository = productRepository;
        _transactionManager = transactionManager;
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<OperationResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return OperationResult<ProductDto>.Failure("Produto não encontrado.", StatusCodes.Status404NotFound);
        }

        return OperationResult<ProductDto>.Success(MapToDto(product), "Produto encontrado com sucesso.");
    }

    public async Task<OperationResult<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            try
            {
                var sku = await GenerateUniqueSkuAsync(innerCancellationToken);
                var product = new Product(request.Name, sku, request.Price, request.StockQty, request.IsActive);
                await _productRepository.AddAsync(product, innerCancellationToken);
                await _productRepository.SaveChangesAsync(innerCancellationToken);

                return OperationResult<ProductDto>.Success(MapToDto(product), "Produto criado com sucesso.", StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return OperationResult<ProductDto>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<ProductDto>.Failure(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }, cancellationToken);
    }

    public async Task<OperationResult<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            var product = await _productRepository.GetTrackedByIdAsync(id, innerCancellationToken);

            if (product is null)
            {
                return OperationResult<ProductDto>.Failure("Produto não encontrado.", StatusCodes.Status404NotFound);
            }

            try
            {
                product.Update(request.Name, request.Price, request.StockQty, request.IsActive);
                await _productRepository.SaveChangesAsync(innerCancellationToken);

                return OperationResult<ProductDto>.Success(MapToDto(product), "Produto atualizado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<ProductDto>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
        }, cancellationToken);
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            var product = await _productRepository.GetTrackedByIdAsync(id, innerCancellationToken);

            if (product is null)
            {
                return OperationResult.Failure("Produto não encontrado.", StatusCodes.Status404NotFound);
            }

            if (await _productRepository.HasRelatedOrderItemsAsync(id, innerCancellationToken))
            {
                return OperationResult.Failure("O produto não pode ser removido porque já está vinculado a pedidos.", StatusCodes.Status409Conflict);
            }

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync(innerCancellationToken);

            return OperationResult.Success("Produto removido com sucesso.");
        }, cancellationToken);
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Sku,
        product.Price,
        product.StockQty,
        product.IsActive,
        product.CreatedAt);

    private async Task<string> GenerateUniqueSkuAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var sku = $"SKU-{Guid.NewGuid():N}";

            if (!await _productRepository.ExistsBySkuAsync(sku, cancellationToken: cancellationToken))
            {
                return sku;
            }
        }

        throw new InvalidOperationException("Não foi possível gerar um SKU único para o produto.");
    }
}
