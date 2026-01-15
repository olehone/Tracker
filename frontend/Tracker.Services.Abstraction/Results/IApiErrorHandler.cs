using Refit;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IApiErrorHandler
{
    Task<Result> ExecuteAsync(Func<Task<ApiResponse<object>>> apiCall);

    Task<Result<TResponse>> ExecuteAsync<TResponse>(Func<Task<ApiResponse<TResponse>>> apiCall);
}