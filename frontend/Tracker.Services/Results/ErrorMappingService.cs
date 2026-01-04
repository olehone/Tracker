using System.Net;
using System.Text.Json;
using Refit;
using Tracker.Domain.Results;

namespace Tracker.Services.Results;

public static class ErrorMappingService
{
    public static Error MapApiResponse<T>(ApiResponse<T> response)
    {
        var statusCode = response.StatusCode;
        var content = response.Error?.Content;

        return statusCode switch
        {
            HttpStatusCode.BadRequest => MapResponseContent(content, ErrorType.Validation),
            HttpStatusCode.Unauthorized => MapResponseContent(content, ErrorType.Unauthorized),
            HttpStatusCode.NotFound => MapResponseContent(content, ErrorType.NotFound),
            HttpStatusCode.Conflict => MapResponseContent(content, ErrorType.Conflict),
            _ when ((int)statusCode >= 500) => MapResponseContent(content, ErrorType.Server),
            _ => new Error(statusCode.ToString(), ErrorType.Unknown)
        };
    }

    private static Error MapResponseContent(string? content, ErrorType errorType)
    {
        var description = TryExtractProblemDetails(content) ?? GetDefaultDescription(errorType);
        return new Error(errorType.ToString(), errorType, description);
    }

    public static Error MapException(Exception exception)
    {
        if (exception is HttpRequestException)
        {
            return new Error(
                "Network.Connection",
                ErrorType.Network,
                GetDefaultDescription(ErrorType.Network)
                );
        }

        if (exception is ValidationApiException validationException)
        {
            return MapValidationException(validationException);
        }

        if (exception is ApiException apiException)
        {
            return apiException.StatusCode switch
            {
                HttpStatusCode.NotFound
                    => MapApiException(apiException, ErrorType.NotFound),

                HttpStatusCode.Conflict
                    => MapApiException(apiException, ErrorType.Conflict),

                HttpStatusCode.Unauthorized
                    => MapApiException(apiException, ErrorType.Unauthorized),

                HttpStatusCode.BadRequest
                    => MapApiException(apiException, ErrorType.Validation),

                _ => MapApiException(apiException, ErrorType.Server)

            };
        }
        return Error.Unknown;
    }

    private static Error MapValidationException(ValidationApiException ex)
    {
        if (ex.Content?.Errors != null && ex.Content.Errors.Count > 0)
        {
            var details = ex.Content.Errors
                .SelectMany(kvp => kvp.Value)
                .ToArray();

            return new Error(
                "Validation",
                ErrorType.Validation,
                ex.Content.Title ?? GetDefaultDescription(ErrorType.Validation),
                details);
        }

        return new Error(
            "Validation",
            ErrorType.Validation,
            ex.Content?.Title ?? GetDefaultDescription(ErrorType.Validation));
    }

    private static Error MapApiException(ApiException ex, ErrorType errorType)
    {
        var description = TryExtractProblemDetails(ex.Content)
            ?? GetDefaultDescription(errorType);

        return new Error(
            ex.StatusCode.ToString(),
            errorType,
            description);
    }

    private static string? TryExtractProblemDetails(string? content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(content);
            return dict?
                .FirstOrDefault(kv => string.Equals(kv.Key, "title", StringComparison.OrdinalIgnoreCase))
                .Value?.ToString();
        }
        catch
        {
            return null;
        }
    }
    private static string GetDefaultDescription(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Validation failed",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "A conflict occurred",
            ErrorType.Unauthorized => "You are not authorized",
            ErrorType.Network => "Unable to connect to the server",
            ErrorType.Server => "Server error occurred",
            _ => "An error occurred"
        };
    }

}