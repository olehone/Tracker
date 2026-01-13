using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardUserService
{
    Task<Result<List<BoardUserDto>>> GetUsersByBoardAsync(GetByIdRequest request);
    Task<Result<BoardUserDto>> AddUserToBoardAsync(AddUserToBoardRequest request);
    Task<Result<BoardUserDto>> ChangeUserRoleAsync(ChangeUserBoardRequest request);
    Task<Result> RemoveUserFromBoardAsync(RemoveUserFromBoardRequest request);
}