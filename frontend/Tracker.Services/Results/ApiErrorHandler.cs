using Refit;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Results;
using Tracker.WebApp.Shared;

namespace Tracker.Services.Results;

public class ApiErrorHandler(IErrorNotifier errorNotifier) : IApiErrorHandler
{

    // Without response body
    // await apiErrorHandler.ExecuteAsync(api.WithoutArgumentsCall);
    // await apiErrorHandler.ExecuteAsync(() => api.WithoutArgumentsCall(arg1, arg2, arg3));
    public async Task<Result> ExecuteAsync(
    Func<Task<IApiResponse>> apiCall)
    {
        try
        {
            var result = await apiCall();
            if (!result.IsSuccessful)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            var error = ErrorMappingService.MapHttpRequestException(ex);
            errorNotifier.NotifyNetworkError(error);
            return error;
        }
    }

    // With typed response body
    // var result = await apiErrorHandler.ExecuteAsync(api.GetAllUsers);
    // var result = await apiErrorHandler.ExecuteAsync(() => api.GetUserById(userId));
    public async Task<Result<TResult>> ExecuteAsync<TResult>(
        Func<Task<IApiResponse<TResult>>> apiCall)
    {
        try
        {
            var result = await apiCall();
            if (!result.IsSuccessful)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.SuccessOf(result.Content!);
        }
        catch (HttpRequestException ex)
        {
            var error = ErrorMappingService.MapHttpRequestException(ex);
            errorNotifier.NotifyNetworkError(error);
            return error;
        }
    }
}