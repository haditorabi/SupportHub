using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Comment comment)
    {
        var result = await _commentService.CreateAsync(comment);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to create comment: {Error}", result.Error);
            return BadRequest(result.Error);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _commentService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Comment comment)
    {
        var result = await _commentService.UpdateAsync(id, comment);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update comment {CommentId}: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commentService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result.Error);
    }
}