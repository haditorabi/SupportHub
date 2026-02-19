using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;
using SupportHub.Infrastructure.Persistence;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CommentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await _db.Comments
            .Include(c => c.Ticket)
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .ToListAsync();
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _db.Comments
            .Include(c => c.Ticket)
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Comment updated)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment is null) return NotFound();

        comment.Body = updated.Body;
        comment.AgentId = updated.AgentId;
        comment.CustomerId = updated.CustomerId;
        await _db.SaveChangesAsync();
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment is null) return NotFound();

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}