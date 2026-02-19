using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;
using SupportHub.Infrastructure.Persistence;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;
    public CustomersController(AppDbContext db) => _db = db;

    // GET api/v1/customers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _db.Customers
            .Include(c => c.Tickets)
            .ToListAsync();
        return Ok(customers);
    }

    // GET api/v1/customers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _db.Customers
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer is null ? NotFound() : Ok(customer);
    }

    // POST api/v1/customers
    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    // PUT api/v1/customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer updated)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        customer.Name = updated.Name;
        customer.Email = updated.Email;
        await _db.SaveChangesAsync();
        return Ok(customer);
    }

    // DELETE api/v1/customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}