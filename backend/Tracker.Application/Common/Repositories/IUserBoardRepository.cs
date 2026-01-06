using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IUserBoardRepository : IRepository<UserBoard, Guid>
{
    Task<UserBoard?> GetByUserAndBoard(Guid userId, Guid boardId);
    Task<UserBoardRole> GetRole(Guid userId, Guid boardId);
}