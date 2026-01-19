using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IUserService
{
    Task<Result<UserDto>> GetCurrentAsync();
    Task<Result<UserDto>> GetByIdAsync(Guid id);
    Task<Result<Paginated<UserDto>>> GetAsync(PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllWorkspacesAsync(
        Guid id, PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        Guid id, PaginatedSearchRequest request);
}