using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardUserService
{
    Task<Result<List<BoardUserDto>>> GetUsersByBoardAsync(Guid boardId);
    Task<Result<BoardUserDto>> AddUserToBoardAsync(Guid boardId, Guid userId, UserBoardRole role);
    Task<Result> ChangeUserRoleAsync(Guid boardId, Guid userId, UserBoardRole role);
    Task<Result> RemoveUserFromBoardAsync(Guid boardId, Guid userId);
}