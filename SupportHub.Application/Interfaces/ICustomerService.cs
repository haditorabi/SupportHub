using SupportHub.Application.Common;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces;

public interface ICustomerService
{
    Task<ServiceResult<Customer>> CreateAsync(Customer customer);
    Task<ServiceResult<Customer>> GetByIdAsync(int id);
    Task<ServiceResult<Customer>> UpdateAsync(int id, Customer updated);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<IEnumerable<Ticket>>> GetTicketsForCustomerAsync(int customerId);
}