using Refit;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IApiErrorHandler
{
    Task<Result> ExecuteAsync<TRequest>(TRequest request, 
        Func<TRequest, Task<ApiResponse<object>>> apiCall);
    Task<Result<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest request, Func<TRequest,
        Task<ApiResponse<TResponse>>> apiCall);
}
