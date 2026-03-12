using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Application.Customers.Dtos;
using backend.src.Application.Customers.Interfaces;
using backend.src.Domain.Entities;

namespace backend.src.Application.Customers.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITransactionManager _transactionManager;

    public CustomerService(ICustomerRepository customerRepository, ITransactionManager transactionManager)
    {
        _customerRepository = customerRepository;
        _transactionManager = transactionManager;
    }

    public async Task<IReadOnlyCollection<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(MapToDto).ToList();
    }

    public async Task<OperationResult<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return OperationResult<CustomerDto>.Failure("Cliente não encontrado.", StatusCodes.Status404NotFound);
        }

        return OperationResult<CustomerDto>.Success(MapToDto(customer), "Cliente encontrado com sucesso.");
    }

    public async Task<OperationResult<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            if (await _customerRepository.ExistsByEmailAsync(request.Email, cancellationToken: innerCancellationToken))
            {
                return OperationResult<CustomerDto>.Failure("Já existe um cliente com o e-mail informado.", StatusCodes.Status409Conflict);
            }

            try
            {
                var customer = new Customer(request.Name, request.Email, request.Document);
                await _customerRepository.AddAsync(customer, innerCancellationToken);
                await _customerRepository.SaveChangesAsync(innerCancellationToken);

                return OperationResult<CustomerDto>.Success(MapToDto(customer), "Cliente criado com sucesso.", StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return OperationResult<CustomerDto>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
        }, cancellationToken);
    }

    public async Task<OperationResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            var customer = await _customerRepository.GetTrackedByIdAsync(id, innerCancellationToken);

            if (customer is null)
            {
                return OperationResult<CustomerDto>.Failure("Cliente não encontrado.", StatusCodes.Status404NotFound);
            }

            if (await _customerRepository.ExistsByEmailAsync(request.Email, id, innerCancellationToken))
            {
                return OperationResult<CustomerDto>.Failure("Já existe um cliente com o e-mail informado.", StatusCodes.Status409Conflict);
            }

            try
            {
                customer.Update(request.Name, request.Email, request.Document);
                await _customerRepository.SaveChangesAsync(innerCancellationToken);

                return OperationResult<CustomerDto>.Success(MapToDto(customer), "Cliente atualizado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<CustomerDto>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
        }, cancellationToken);
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async innerCancellationToken =>
        {
            var customer = await _customerRepository.GetTrackedByIdAsync(id, innerCancellationToken);

            if (customer is null)
            {
                return OperationResult.Failure("Cliente não encontrado.", StatusCodes.Status404NotFound);
            }

            if (await _customerRepository.HasRelatedOrdersAsync(id, innerCancellationToken))
            {
                return OperationResult.Failure("O cliente não pode ser removido porque já possui pedidos vinculados.", StatusCodes.Status409Conflict);
            }

            _customerRepository.Remove(customer);
            await _customerRepository.SaveChangesAsync(innerCancellationToken);

            return OperationResult.Success("Cliente removido com sucesso.");
        }, cancellationToken);
    }

    private static CustomerDto MapToDto(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Email,
        customer.Document,
        customer.CreatedAt);
}
