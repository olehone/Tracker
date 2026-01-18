using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IUserBoardRepository : IRepository<UserBoard, Guid>
{
    Task<UserBoard?> GetByUserAndBoardAsync(Guid userId, Guid boardId);
    Task<IReadOnlyList<UserBoard>> GetByBoardAsync(Guid boardId);
    Task<UserBoard?> GetOwnerAsync(Guid boardId);
    Task<UserBoardRole> GetRoleAsync(Guid userId, Guid boardId);
}