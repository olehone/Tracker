using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IBoardUserRepository : IRepository<BoardUser, Guid>
{
    Task<BoardUser?> GetAsync(Guid userId, Guid boardId);
    Task<IReadOnlyList<BoardUser>> GetAsync(Guid boardId);
    Task<BoardUser?> GetOwnerAsync(Guid boardId);
    Task<BoardUserRole> GetRoleAsync(Guid userId, Guid boardId);
}