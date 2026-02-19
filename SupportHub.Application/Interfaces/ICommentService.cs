using SupportHub.Application.Common;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces;

public interface ICommentService
{
    Task<ServiceResult<Comment>> CreateAsync(Comment comment);
    Task<ServiceResult<Comment>> GetByIdAsync(int id);
    Task<ServiceResult<Comment>> UpdateAsync(int id, Comment updated);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}