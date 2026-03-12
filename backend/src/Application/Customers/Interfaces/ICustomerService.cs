using backend.src.Application.Common.Models;
using backend.src.Application.Customers.Dtos;

namespace backend.src.Application.Customers.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyCollection<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OperationResult<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
