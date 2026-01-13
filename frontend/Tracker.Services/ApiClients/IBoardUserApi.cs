using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IBoardUserApi
{
    [Get("/api/boards-users/{boardId}")]
    Task<ApiResponse<List<BoardUserDto>>> GetUsersByBoardAsync(Guid boardId);
    [Post("/api/boards-users")]
    Task<ApiResponse<BoardUserDto>> AddUserToBoardAsync(AddUserToBoardRequest request);
    [Put("/api/boards-users")]
    Task<ApiResponse<object>> ChangeUserRoleAsync(ChangeUserBoardRequest request);
    [Delete("/api/boards-users")]
    Task<ApiResponse<object>> RemoveUserFromBoardAsync(RemoveUserFromBoardRequest request);
}