using Refit;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Results;
using Tracker.WebApp.Shared;

namespace Tracker.Services.Results;

public class ApiErrorHandler(IErrorNotifier errorNotifier) : IApiErrorHandler
{
    // Without request without response 
    // Result ApiCall()
    public async Task<Result> ExecuteAsync(
    Func<Task<ApiResponse<object>>> apiCall)
    {
        try
        {
            var result = await apiCall();
            if (!result.IsSuccessStatusCode)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            return ErrorMappingService.MapHttpRequestException(ex);
        }
    }

    // Without request with response 
    // Result<Object> ApiCall()
    public async Task<Result<TResult>> ExecuteAsync<TResult>(
        Func<Task<ApiResponse<TResult>>> apiCall)
    {
        try
        {
            var result = await apiCall();
            if (!result.IsSuccessStatusCode)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.SuccessOf(result.Content!);
        }
        catch (HttpRequestException ex)
        {
            return ErrorMappingService.MapHttpRequestException(ex);
        }
    }

    // With request without response 
    // Result ApiCall(Request request)
    public async Task<Result> ExecuteAsync<TRequest>(
        TRequest request,
        Func<TRequest, Task<ApiResponse<object>>> apiCall)
    {
        try
        {
            var result = await apiCall(request);
            if (!result.IsSuccessStatusCode)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            return ErrorMappingService.MapHttpRequestException(ex);
        }
    }

    // With request with response
    // Result<Object> ApiCall(Request request)
    public async Task<Result<TResult>> ExecuteAsync<TRequest, TResult>(
        TRequest request,
        Func<TRequest, Task<ApiResponse<TResult>>> apiCall)
    {
        try
        {
            var result = await apiCall(request);
            if (!result.IsSuccessStatusCode)
            {
                var error = ErrorMappingService.MapApiResponse(result);
                errorNotifier.NotifyActionError(error);
                return error;
            }

            return Result.SuccessOf(result.Content!);
        }
        catch (HttpRequestException ex)
        {
            return ErrorMappingService.MapHttpRequestException(ex);
        }
    }
}