using backend.src.Application.Common.Models;
using backend.src.Application.Orders.Dtos;

namespace backend.src.Application.Orders.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyCollection<OrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OperationResult<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult<OrderDto>> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
}
