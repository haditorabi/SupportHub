using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Ticket ticket)
    {
        var result = await _ticketService.CreateAsync(ticket);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to create ticket: {Error}", result.Error);
            return BadRequest(result.Error);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _ticketService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Ticket ticket)
    {
        var result = await _ticketService.UpdateAsync(id, ticket);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update ticket {TicketId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _ticketService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result.Error);
    }

    [HttpPost("{ticketId}/assign/{agentId}")]
    public async Task<IActionResult> AssignAgent(int ticketId, int agentId)
    {
        var result = await _ticketService.AssignAgentAsync(ticketId, agentId);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to assign agent {AgentId} to ticket {TicketId}: {Error}", agentId, ticketId, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TicketStatus status)
    {
        var result = await _ticketService.UpdateStatusAsync(id, status);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update status for ticket {TicketId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpPatch("{id}/category")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] TicketCategory category)
    {
        var result = await _ticketService.UpdateCategoryAsync(id, category);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update category for ticket {TicketId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpPatch("{id}/priority")]
    public async Task<IActionResult> UpdatePriority(int id, [FromBody] TicketPriority priority)
    {
        var result = await _ticketService.UpdatePriorityAsync(id, priority);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update priority for ticket {TicketId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }
}