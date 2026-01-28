using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardUser;

namespace Tracker.Services.ApiClients;

public interface IBoardUserApi
{
    [Post("/api/boards/{boardId}/users/{userId}")]
    Task<IApiResponse<BoardUserDto>> AddAsync(Guid boardId, Guid userId,
        BoardUserRoleRequest request);

    [Put("/api/boards/{boardId}/users/{userId}")]
    Task<IApiResponse> ChangeRoleAsync(Guid boardId, Guid userId,
        BoardUserRoleRequest request);

    [Delete("/api/boards/{boardId}/users/{userId}")]
    Task<IApiResponse> RemoveAsync(Guid boardId, Guid userId);
}