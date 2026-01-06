using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IUserBoardRepository : IRepository<UserBoard, Guid>
{
    Task<UserBoard?> GetByUserAndBoard(Guid userId, Guid boardId);
}