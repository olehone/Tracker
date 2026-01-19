using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardUserService
{
    Task<Result<List<BoardUserDto>>> GetByBoardAsync(Guid boardId);
    Task<Result<BoardUserDto>> AddAsync(Guid boardId, Guid userId, UserBoardRole role);
    Task<Result> ChangeRoleAsync(Guid boardId, Guid userId, UserBoardRole role);
    Task<Result> RemoveAsync(Guid boardId, Guid userId);
}