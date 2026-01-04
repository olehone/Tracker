using Refit;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Results;

namespace Tracker.Services.Results;

public class ApiErrorHandler : IApiErrorHandler
{
    public async Task<Result> ExecuteAsync<TRequest>(
        TRequest request,
        Func<TRequest, Task<ApiResponse<object>>> apiCall)
    {
        try
        {
            var result = await apiCall(request);
            if (!result.IsSuccessStatusCode)
            {
                return ErrorMappingService.MapApiResponse(result);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return ErrorMappingService.MapException(ex);
        }
    }

    public async Task<Result<TResult>> ExecuteAsync<TRequest, TResult>(
        TRequest request,
        Func<TRequest, Task<ApiResponse<TResult>>> apiCall)
    {
        try
        {
            var result = await apiCall(request);
            if (!result.IsSuccessStatusCode)
            {
                return ErrorMappingService.MapApiResponse(result);
            }

            if(result.Content is null)
            {
                return Result.FailureOf<TResult>(Error.Unknown);
            }
            return Result.SuccessOf(result.Content);
        }
        catch (Exception ex)
        {
            return ErrorMappingService.MapException(ex);
        }
    }
}