using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;
using SupportHub.Infrastructure.Persistence;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgentsController: ControllerBase
{
    private readonly AppDbContext _db;
    public AgentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var agents = await _db.Agents
            .Include(a => a.Tickets)
            .ToListAsync();
        return Ok(agents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var agent = await _db.Agents
            .Include(a => a.Tickets)
            .FirstOrDefaultAsync(a => a.Id == id);

        return agent is null ? NotFound() : Ok(agent);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Agent agent)
    {
        agent.CreatedAt = DateTime.UtcNow;
        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = agent.Id }, agent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Agent updated)
    {
        var agent = await _db.Agents.FindAsync(id);
        if (agent is null) return NotFound();

        agent.DisplayName = updated.DisplayName;
        agent.Email = updated.Email;
        await _db.SaveChangesAsync();
        return Ok(agent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var agent = await _db.Agents.FindAsync(id);
        if (agent is null) return NotFound();

        _db.Agents.Remove(agent);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}