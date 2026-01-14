using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardUserService(IApiErrorHandler apiErrorHandler, IBoardUserApi api) : IBoardUserService
{
    public Task<Result<List<BoardUserDto>>> GetUsersByBoardAsync(Guid boardId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetUsersByBoardAsync(boardId));
    }

    public Task<Result<BoardUserDto>> AddUserToBoardAsync(Guid boardId, Guid userId, UserBoardRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.AddUserToBoardAsync(boardId, userId, role));
    }

    public Task<Result> ChangeUserRoleAsync(Guid boardId, Guid userId, UserBoardRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.ChangeUserRoleAsync(boardId, userId, role));
    }

    public Task<Result> RemoveUserFromBoardAsync(Guid boardId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.RemoveUserFromBoardAsync(boardId, userId));
    }
}