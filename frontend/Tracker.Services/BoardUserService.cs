using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardUserService(IApiErrorHandler apiErrorHandler, IBoardUserApi api) : IBoardUserService
{
    public Task<Result<List<BoardUserDto>>> GetByBoardAsync(Guid boardId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetByBoardAsync(boardId));
    }

    public Task<Result<BoardUserDto>> AddAsync(Guid boardId, Guid userId, UserBoardRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.AddAsync(boardId, userId, role));
    }

    public Task<Result> ChangeRoleAsync(Guid boardId, Guid userId, UserBoardRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.ChangeRoleAsync(boardId, userId, role));
    }

    public Task<Result> RemoveAsync(Guid boardId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.RemoveAsync(boardId, userId));
    }
}