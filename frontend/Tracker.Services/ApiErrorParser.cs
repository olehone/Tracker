using System.Net;
using Refit;

namespace Tracker.Services;

public static class ApiErrorParser
{
    public static List<string> ToMessages(ApiResponse<object> response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return ["You must be logged in to continue."];

        if (response.StatusCode == HttpStatusCode.Forbidden)
            return ["You do not have permission to perform this action."];

        if (response.StatusCode == HttpStatusCode.NotFound)
            return ["The requested resource was not found."];


        return ["Request failed. Please try again."];
    }
}

