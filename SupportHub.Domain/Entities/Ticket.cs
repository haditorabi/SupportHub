using SupportHub.Domain.Enums;

namespace SupportHub.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }

    // FK - Customer (required)
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; } = null!;

    // FK - Agent (optional/nullable)
    public int? AssignedAgentId { get; set; }
    public Agent? AssignedAgent { get; set; }

    public TicketCategory Category { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}