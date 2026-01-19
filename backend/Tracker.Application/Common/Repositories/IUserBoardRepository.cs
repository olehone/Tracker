using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IUserBoardRepository : IRepository<UserBoard, Guid>
{
    Task<UserBoard?> GetAsync(Guid userId, Guid boardId);
    Task<IReadOnlyList<UserBoard>> GetAsync(Guid boardId);
    Task<UserBoard?> GetOwnerAsync(Guid boardId);
    Task<UserBoardRole> GetRoleAsync(Guid userId, Guid boardId);
}