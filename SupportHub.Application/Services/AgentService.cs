using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Common;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Services;

public class AgentService : IAgentService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<AgentService> _logger;

    public AgentService(IAppDbContext db, ILogger<AgentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ServiceResult<Agent>> CreateAsync(Agent agent)
    {
        if (string.IsNullOrWhiteSpace(agent.DisplayName))
            return ServiceResult<Agent>.Fail("DisplayName is required.");

        if (string.IsNullOrWhiteSpace(agent.Email))
            return ServiceResult<Agent>.Fail("Email is required.");

        var emailTaken = await _db.Agents.AnyAsync(a => a.Email == agent.Email);
        if (emailTaken)
            return ServiceResult<Agent>.Fail("An agent with that email already exists.");

        agent.CreatedAt = DateTime.UtcNow;
        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Agent created with Id {AgentId}", agent.Id);
        return ServiceResult<Agent>.Ok(agent);
    }

    public async Task<ServiceResult<Agent>> GetByIdAsync(int id)
    {
        var agent = await _db.Agents
            .Include(a => a.Tickets)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agent is null)
        {
            _logger.LogWarning("Agent with Id {AgentId} not found", id);
            return ServiceResult<Agent>.Fail("Agent not found.");
        }

        return ServiceResult<Agent>.Ok(agent);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var agent = await _db.Agents.FindAsync(id);
        if (agent is null)
            return ServiceResult<bool>.Fail("Agent not found.");

        _db.Agents.Remove(agent);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Agent {AgentId} deleted", id);
        return ServiceResult<bool>.Ok(true);
    }
}