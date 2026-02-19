using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Common;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IAppDbContext db, ILogger<CustomerService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ServiceResult<Customer>> CreateAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            return ServiceResult<Customer>.Fail("Name is required.");

        if (string.IsNullOrWhiteSpace(customer.Email))
            return ServiceResult<Customer>.Fail("Email is required.");

        var emailTaken = await _db.Customers
            .AnyAsync(c => c.Email == customer.Email);

        if (emailTaken)
            return ServiceResult<Customer>.Fail("A customer with that email already exists.");

        customer.CreatedAt = DateTime.UtcNow;
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Customer created with Id {CustomerId}", customer.Id);
        return ServiceResult<Customer>.Ok(customer);
    }

    public async Task<ServiceResult<Customer>> GetByIdAsync(int id)
    {
        var customer = await _db.Customers
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            _logger.LogWarning("Customer with Id {CustomerId} not found", id);
            return ServiceResult<Customer>.Fail("Customer not found.");
        }

        return ServiceResult<Customer>.Ok(customer);
    }

    public async Task<ServiceResult<Customer>> UpdateAsync(int id, Customer updated)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null)
            return ServiceResult<Customer>.Fail("Customer not found.");

        if (string.IsNullOrWhiteSpace(updated.Name))
            return ServiceResult<Customer>.Fail("Name is required.");

        if (string.IsNullOrWhiteSpace(updated.Email))
            return ServiceResult<Customer>.Fail("Email is required.");

        var emailTaken = await _db.Customers
            .AnyAsync(c => c.Email == updated.Email && c.Id != id);

        if (emailTaken)
            return ServiceResult<Customer>.Fail("Email is already in use by another customer.");

        customer.Name = updated.Name;
        customer.Email = updated.Email;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Customer {CustomerId} updated", id);
        return ServiceResult<Customer>.Ok(customer);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null)
            return ServiceResult<bool>.Fail("Customer not found.");

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Customer {CustomerId} deleted", id);
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<IEnumerable<Ticket>>> GetTicketsForCustomerAsync(int customerId)
    {
        var exists = await _db.Customers.AnyAsync(c => c.Id == customerId);
        if (!exists)
            return ServiceResult<IEnumerable<Ticket>>.Fail("Customer not found.");

        var tickets = await _db.Tickets
            .Include(t => t.AssignedAgent)
            .Include(t => t.Comments)
            .Where(t => t.CustomerId == customerId)
            .ToListAsync();

        return ServiceResult<IEnumerable<Ticket>>.Ok(tickets);
    }
}