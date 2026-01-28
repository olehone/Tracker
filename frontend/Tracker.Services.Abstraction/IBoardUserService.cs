using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardUserService
{
    Task<Result<BoardUserDto>> AddAsync(Guid boardId, Guid userId, BoardUserRole role);
    Task<Result> ChangeRoleAsync(Guid boardId, Guid userId, BoardUserRole role);
    Task<Result> RemoveAsync(Guid boardId, Guid userId);
}