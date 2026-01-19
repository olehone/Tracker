using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IBoardUserApi
{
    [Get("/api/boards/{boardId}/users")]
    Task<ApiResponse<List<BoardUserDto>>> GetByBoardAsync(Guid boardId);

    [Post("/api/boards/{boardId}/users/{userId}")]
    Task<ApiResponse<BoardUserDto>> AddAsync(Guid boardId, Guid userId,
        BoardUserRoleRequest request);

    [Put("/api/boards/{boardId}/users/{userId}")]
    Task<ApiResponse<object>> ChangeRoleAsync(Guid boardId, Guid userId,
        BoardUserRoleRequest request);

    [Delete("/api/boards/{boardId}/users/{userId}")]
    Task<ApiResponse<object>> RemoveAsync(Guid boardId, Guid userId);
}