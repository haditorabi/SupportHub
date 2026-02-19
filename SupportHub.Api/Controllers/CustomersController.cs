using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        var result = await _customerService.CreateAsync(customer);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to create customer: {Error}", result.Error);
            return BadRequest(result.Error);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        var result = await _customerService.UpdateAsync(id, customer);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update customer {CustomerId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result.Error);
    }

    [HttpGet("{id}/tickets")]
    public async Task<IActionResult> GetTickets(int id)
    {
        var result = await _customerService.GetTicketsForCustomerAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }
}