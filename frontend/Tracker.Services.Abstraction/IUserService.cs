using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.Users;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IUserService
{
    Task<Result<UserDto>> GetByIdAsync(Guid id);
    Task<Result> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<Result<UserDto>> GetCurrentAsync();
    Task<Result<Paginated<UserDto>>> GetAsync(PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllWorkspacesAsync(
        Guid id, PaginatedSearchRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        Guid id, PaginatedSearchRequest request);
    Task<Result<string>> UploadAvatarAsync(Guid userId, Stream fileStream, string contentType, string fileName);
    Task<Result> DeleteAvatarAsync(Guid userId);
}