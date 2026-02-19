using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Agent> Agents { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<Comment> Comments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}