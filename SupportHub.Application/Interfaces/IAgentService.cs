using SupportHub.Application.Common;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces;

public interface IAgentService
{
    Task<ServiceResult<Agent>> CreateAsync(Agent agent);
    Task<ServiceResult<Agent>> GetByIdAsync(int id);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}