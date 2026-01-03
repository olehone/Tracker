using Microsoft.AspNetCore.Mvc;
using Tracker.Domain.Results;

namespace Tracker.API.Services;

public static class ErrorMappingService
{
    public static IActionResult ToActionResult(this Result response)
    {
        return ResultToActionResult(response);
    }

    public static IActionResult ToActionResult<TValue>(this Result<TValue> response)
    {
        return ResultToActionResult(response);
    }
    public static IActionResult ResultToActionResult(Result response)
    {
        return response.IsSuccess
            ? new NoContentResult()
            : response.Error.ToProblemDetails();

    }

    public static IActionResult ResultToActionResult<TValue>(Result<TValue> response)
    {
        return response.IsSuccess
            ? new OkObjectResult(response.Value)
            : response.Error.ToProblemDetails();
    }

    public static IActionResult ToProblemDetails(this Error error)
    {
        return ErrorToProblemDetails(error);
    }

    public static IActionResult ErrorToProblemDetails(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(new HttpValidationProblemDetails
            {
                Title = error.Description,
                Status = StatusCodes.Status400BadRequest,
                Errors = new Dictionary<string, string[]>
                {
                    ["ValidationErrors"] = error.Details!
                }
            }),

            ErrorType.NotFound => new NotFoundObjectResult(new ProblemDetails
            {
                Title = error.Description,
                Status = StatusCodes.Status404NotFound
            }),

            ErrorType.Conflict => new ConflictObjectResult(new ProblemDetails
            {
                Title = error.Description,
                Status = StatusCodes.Status409Conflict
            }),

            _ => new BadRequestObjectResult(new ProblemDetails
            {
                Title = error.Description,
                Status = StatusCodes.Status400BadRequest
            })
        };
    }
}