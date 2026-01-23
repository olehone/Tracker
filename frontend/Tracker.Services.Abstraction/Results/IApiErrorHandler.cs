using Refit;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IApiErrorHandler
{
    Task<Result> ExecuteAsync(Func<Task<IApiResponse>> apiCall);

    Task<Result<TResponse>> ExecuteAsync<TResponse>(Func<Task<IApiResponse<TResponse>>> apiCall);
}