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
            _ => new Error(statusCode.ToString(), ErrorType.Unknown, "Unknown error")
        };
    }

    private static Error MapResponseContent(string? content, ErrorType errorType)
    {
        if (errorType is ErrorType.Validation)
        {
            return MapValidationResponseContent(content);
        }

        var description = TryExtractProblemDetails(content) ?? GetDefaultDescription(errorType);
        return new Error(errorType.ToString(), errorType, description);
    }

    public static Error MapHttpRequestException(HttpRequestException exception)
    {
        return new Error(
            "Network.Connection",
            ErrorType.Network,
            GetDefaultDescription(ErrorType.Network)
            );
    }

    private static Error MapValidationResponseContent(string? content)
    {
        const string Code = "Validation";
        if (string.IsNullOrWhiteSpace(content))
        {
            return new Error(
                Code,
                ErrorType.Validation,
                GetDefaultDescription(ErrorType.Validation));
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var title = root.TryGetProperty("title", out var titleProp)
                ? titleProp.GetString()
                : GetDefaultDescription(ErrorType.Validation);

            if (root.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == JsonValueKind.Object)
            {
                var details = errorsProp
                    .EnumerateObject()
                    .SelectMany(p =>
                        p.Value.ValueKind == JsonValueKind.Array
                            ? p.Value.EnumerateArray().Select(v => v.GetString())
                            : Array.Empty<string?>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Cast<string>()
                    .ToArray();

                return new Error(
                    Code,
                    ErrorType.Validation,
                    title ?? GetDefaultDescription(ErrorType.Validation),
                    details);
            }

            return new Error(
                Code,
                ErrorType.Validation,
                title ?? GetDefaultDescription(ErrorType.Validation));
        }
        catch
        {
            return new Error(
                Code,
                ErrorType.Validation,
                GetDefaultDescription(ErrorType.Validation));
        }
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