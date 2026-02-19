namespace SupportHub.Domain.Entities;

public class Agent
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}