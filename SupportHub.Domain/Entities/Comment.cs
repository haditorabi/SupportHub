namespace SupportHub.Domain.Entities;

public class Comment
{
    public int Id { get; set; }

    // FK - Ticket (required)
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    // FK - Agent (nullable - one of these two will be set)
    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }

    // FK - Customer (nullable - one of these two will be set)
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}