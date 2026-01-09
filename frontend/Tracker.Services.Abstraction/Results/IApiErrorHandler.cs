using Refit;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IApiErrorHandler
{
    Task<Result> ExecuteAsync(Func<Task<ApiResponse<object>>> apiCall);

    Task<Result<TResponse>> ExecuteAsync<TResponse>(Func<Task<ApiResponse<TResponse>>> apiCall);

    Task<Result> ExecuteAsync<TRequest>(TRequest request,
        Func<TRequest, Task<ApiResponse<object>>> apiCall);

    Task<Result<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest request, Func<TRequest,
        Task<ApiResponse<TResponse>>> apiCall);

    Task<Result> ExecuteAsync<TId, TRequest>(TId idRequest, TRequest request, 
        Func<TId, TRequest, Task<ApiResponse<object>>> apiCall);

    Task<Result<TResponse>> ExecuteAsync<TId, TRequest, TResponse>(TId idRequest, TRequest request, 
        Func<TId, TRequest, Task<ApiResponse<TResponse>>> apiCall);
}