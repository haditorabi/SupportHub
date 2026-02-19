using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ILogger<AgentsController> _logger;

    public AgentsController(IAgentService agentService, ILogger<AgentsController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Agent agent)
    {
        var result = await _agentService.CreateAsync(agent);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to create agent: {Error}", result.Error);
            return BadRequest(result.Error);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _agentService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _agentService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result.Error);
    }
}