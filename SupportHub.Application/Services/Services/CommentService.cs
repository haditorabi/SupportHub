using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Common;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Services;

public class CommentService : ICommentService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CommentService> _logger;

    public CommentService(IAppDbContext db, ILogger<CommentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ServiceResult<Comment>> CreateAsync(Comment comment)
    {
        if (string.IsNullOrWhiteSpace(comment.Body))
            return ServiceResult<Comment>.Fail("Comment body cannot be empty.");

        var ticket = await _db.Tickets.FindAsync(comment.TicketId);
        if (ticket is null)
            return ServiceResult<Comment>.Fail("The specified ticket does not exist.");

        if (ticket.Status == TicketStatus.Closed)
            return ServiceResult<Comment>.Fail("Cannot add comments to a closed ticket.");

        // If authored by a customer, verify they own the ticket
        if (comment.CustomerId.HasValue)
        {
            if (comment.CustomerId.Value != ticket.CustomerId)
                return ServiceResult<Comment>.Fail("A customer can only comment on their own tickets.");
        }

        comment.CreatedAt = DateTime.UtcNow;
        _db.Comments.Add(comment);

        // Update ticket's UpdatedAt
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment {CommentId} added to Ticket {TicketId}", comment.Id, comment.TicketId);
        return ServiceResult<Comment>.Ok(comment);
    }

    public async Task<ServiceResult<Comment>> GetByIdAsync(int id)
    {
        var comment = await _db.Comments
            .Include(c => c.Ticket)
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null)
        {
            _logger.LogWarning("Comment {CommentId} not found", id);
            return ServiceResult<Comment>.Fail("Comment not found.");
        }

        return ServiceResult<Comment>.Ok(comment);
    }

    public async Task<ServiceResult<Comment>> UpdateAsync(int id, Comment updated)
    {
        var comment = await _db.Comments
            .Include(c => c.Ticket)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null)
            return ServiceResult<Comment>.Fail("Comment not found.");

        if (string.IsNullOrWhiteSpace(updated.Body))
            return ServiceResult<Comment>.Fail("Comment body cannot be empty.");

        if (comment.Ticket.Status == TicketStatus.Closed)
            return ServiceResult<Comment>.Fail("Cannot edit comments on a closed ticket.");

        comment.Body = updated.Body;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment {CommentId} updated", id);
        return ServiceResult<Comment>.Ok(comment);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment is null)
            return ServiceResult<bool>.Fail("Comment not found.");

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment {CommentId} deleted", id);
        return ServiceResult<bool>.Ok(true);
    }
}