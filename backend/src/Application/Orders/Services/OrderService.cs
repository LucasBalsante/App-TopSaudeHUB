using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Application.Orders.Dtos;
using backend.src.Application.Orders.Interfaces;
using backend.src.Domain.Entities;

namespace backend.src.Application.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ITransactionManager _transactionManager;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ITransactionManager transactionManager)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _transactionManager = transactionManager;
    }

    public async Task<OperationResult<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return OperationResult<OrderDto>.Failure("Pedido não encontrado.", StatusCodes.Status404NotFound);
        }

        return OperationResult<OrderDto>.Success(MapToDto(order), "Pedido encontrado com sucesso.");
    }

    public async Task<OperationResult<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            if (request.Items.Count == 0)
            {
                return OperationResult<OrderDto>.Failure("O pedido deve possuir ao menos um item.", StatusCodes.Status400BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, innerCancellationToken);
            if (customer is null)
            {
                return OperationResult<OrderDto>.Failure("Cliente não encontrado.", StatusCodes.Status404NotFound);
            }

            var groupedItems = request.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity)
                })
                .ToList();

            if (groupedItems.Any(item => item.Quantity <= 0))
            {
                return OperationResult<OrderDto>.Failure("A quantidade dos itens do pedido deve ser maior que zero.", StatusCodes.Status400BadRequest);
            }

            var products = await _productRepository.GetTrackedByIdsAsync(groupedItems.Select(item => item.ProductId), innerCancellationToken);
            var productsById = products.ToDictionary(product => product.Id);

            foreach (var item in groupedItems)
            {
                if (!productsById.TryGetValue(item.ProductId, out var product))
                {
                    return OperationResult<OrderDto>.Failure($"Produto {item.ProductId} não encontrado.", StatusCodes.Status404NotFound);
                }

                if (!product.IsActive)
                {
                    return OperationResult<OrderDto>.Failure($"O produto {product.Sku} está inativo para venda.", StatusCodes.Status409Conflict);
                }

                if (product.StockQty < item.Quantity)
                {
                    return OperationResult<OrderDto>.Failure($"Estoque insuficiente para o produto {product.Sku}.", StatusCodes.Status409Conflict);
                }
            }

            try
            {
                var order = new Order(request.CustomerId);

                foreach (var item in groupedItems)
                {
                    var product = productsById[item.ProductId];
                    product.DecreaseStock(item.Quantity);
                    order.AddItem(product, item.Quantity);
                }

                await _orderRepository.AddAsync(order, innerCancellationToken);
                await _orderRepository.SaveChangesAsync(innerCancellationToken);

                var createdOrder = await _orderRepository.GetByIdAsync(order.Id, innerCancellationToken) ?? order;
                return OperationResult<OrderDto>.Success(MapToDto(createdOrder), "Pedido criado com sucesso.", StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return OperationResult<OrderDto>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
        }, cancellationToken);
    }

    private static OrderDto MapToDto(Order order) => new(
        order.Id,
        order.CustomerId,
        order.TotalAmount,
        order.Status,
        order.CreatedAt,
        order.OrderItems
            .Select(item => new OrderItemDto(
                item.ProductId,
                item.Product?.Name ?? string.Empty,
                item.Product?.Sku ?? string.Empty,
                item.UnitPrice,
                item.Quantity,
                item.LineTotal))
            .ToList());
}
