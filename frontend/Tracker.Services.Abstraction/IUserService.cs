using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IUserService
{
    Task<Result<UserDto>> GetCurrentUserAsync();
    Task<Result<UserDto>> GetUserByIdAsync(GetByIdRequest request);
    Task<Result<Paginated<UserDto>>> GetUsersAsync(PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllUserWorkspacesAsync(
        GetByIdRequest id, PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        GetByIdRequest id, PaginatedSearchRequest request);
}