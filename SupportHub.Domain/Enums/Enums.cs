namespace SupportHub.Domain.Enums;

public enum TicketCategory
{
    Billing = 1,
    Performance = 2,
    Access = 3,
    Bug = 4,
    FeatureRequest = 5,
    Other = 6
}

public enum TicketStatus
{
    Open = 1,
    InProgress = 2,
    WaitingOnCustomer = 3,
    Resolved = 4,
    Closed = 5,
}

public enum TicketPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}