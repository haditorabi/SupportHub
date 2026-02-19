using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Common;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Services;

public class TicketService : ITicketService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<TicketService> _logger;

    public TicketService(IAppDbContext db, ILogger<TicketService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ServiceResult<Ticket>> CreateAsync(Ticket ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket.Title))
            return ServiceResult<Ticket>.Fail("Title is required.");

        if (string.IsNullOrWhiteSpace(ticket.Description))
            return ServiceResult<Ticket>.Fail("Description is required.");

        var customerExists = await _db.Customers.AnyAsync(c => c.Id == ticket.CustomerId);
        if (!customerExists)
            return ServiceResult<Ticket>.Fail("The specified customer does not exist.");

        ticket.Status = TicketStatus.Open;
        ticket.Priority = TicketPriority.Medium;
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.AssignedAgentId = null;

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} created for Customer {CustomerId}", ticket.Id, ticket.CustomerId);
        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<Ticket>> GetByIdAsync(int id)
    {
        var ticket = await _db.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedAgent)
            .Include(t => t.Comments)
                .ThenInclude(c => c.Agent)
            .Include(t => t.Comments)
                .ThenInclude(c => c.Customer)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket is null)
        {
            _logger.LogWarning("Ticket {TicketId} not found", id);
            return ServiceResult<Ticket>.Fail("Ticket not found.");
        }

        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<Ticket>> UpdateAsync(int id, Ticket updated)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null)
            return ServiceResult<Ticket>.Fail("Ticket not found.");

        if (string.IsNullOrWhiteSpace(updated.Title))
            return ServiceResult<Ticket>.Fail("Title is required.");

        if (string.IsNullOrWhiteSpace(updated.Description))
            return ServiceResult<Ticket>.Fail("Description is required.");

        ticket.Title = updated.Title;
        ticket.Description = updated.Description;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} updated", id);
        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null)
            return ServiceResult<bool>.Fail("Ticket not found.");

        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} deleted", id);
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<Ticket>> AssignAgentAsync(int ticketId, int agentId)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null)
            return ServiceResult<Ticket>.Fail("Ticket not found.");

        if (ticket.Status == TicketStatus.Closed)
            return ServiceResult<Ticket>.Fail("Cannot assign an agent to a closed ticket.");

        var agent = await _db.Agents.FindAsync(agentId);
        if (agent is null)
            return ServiceResult<Ticket>.Fail("Agent not found.");

        ticket.AssignedAgentId = agentId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Agent {AgentId} assigned to Ticket {TicketId}", agentId, ticketId);
        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<Ticket>> UpdateStatusAsync(int id, TicketStatus status)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null)
            return ServiceResult<Ticket>.Fail("Ticket not found.");

        ticket.Status = status;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} status changed to {Status}", id, status);
        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<Ticket>> UpdateCategoryAsync(int id, TicketCategory category)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null)
            return ServiceResult<Ticket>.Fail("Ticket not found.");

        if (ticket.Status is TicketStatus.Resolved or TicketStatus.Closed)
            return ServiceResult<Ticket>.Fail("Cannot change category of a resolved or closed ticket.");

        ticket.Category = category;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} category changed to {Category}", id, category);
        return ServiceResult<Ticket>.Ok(ticket);
    }

    public async Task<ServiceResult<Ticket>> UpdatePriorityAsync(int id, TicketPriority priority)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket is null)
            return ServiceResult<Ticket>.Fail("Ticket not found.");

        if (ticket.Status is TicketStatus.Resolved or TicketStatus.Closed)
            return ServiceResult<Ticket>.Fail("Cannot change priority of a resolved or closed ticket.");

        ticket.Priority = priority;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} priority changed to {Priority}", id, priority);
        return ServiceResult<Ticket>.Ok(ticket);
    }
}