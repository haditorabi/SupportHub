using SupportHub.Application.Common;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Interfaces;

public interface ITicketService
{
    Task<ServiceResult<Ticket>> CreateAsync(Ticket ticket);
    Task<ServiceResult<Ticket>> GetByIdAsync(int id);
    Task<ServiceResult<Ticket>> UpdateAsync(int id, Ticket updated);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<Ticket>> AssignAgentAsync(int ticketId, int agentId);
    Task<ServiceResult<Ticket>> UpdateStatusAsync(int id, TicketStatus status);
    Task<ServiceResult<Ticket>> UpdateCategoryAsync(int id, TicketCategory category);
    Task<ServiceResult<Ticket>> UpdatePriorityAsync(int id, TicketPriority priority);
}