using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardUserService(IApiErrorHandler apiErrorHandler, IBoardUserApi api) : IBoardUserService
{
    public Task<Result<List<BoardUserDto>>> GetUsersByBoardAsync(GetByIdRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request.Id, api.GetUsersByBoardAsync);
    }

    public Task<Result<BoardUserDto>> AddUserToBoardAsync(AddUserToBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.AddUserToBoardAsync);
    }

    public Task<Result<BoardUserDto>> ChangeUserRoleAsync(ChangeUserBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.ChangeUserRoleAsync);
    }

    public Task<Result> RemoveUserFromBoardAsync(RemoveUserFromBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.RemoveUserFromBoardAsync);
    }
}