using backend.src.Application.Common.Models;
using backend.src.Application.Orders.Dtos;

namespace backend.src.Application.Orders.Interfaces;

public interface IOrderService
{
    Task<OperationResult<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OperationResult<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
