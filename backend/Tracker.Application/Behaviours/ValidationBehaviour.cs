using FluentValidation;
using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.Behaviours;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }
        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }
        var error = Error.Validation(
            failures.Select(x => x.ErrorMessage).ToArray()
        );

        return FailureResponse<TResponse>(error);
    }

    private static TResponse FailureResponse<TResponse>(Error error) where TResponse : Result
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }
        var genericResultType = typeof(TResponse);
        if (genericResultType.IsGenericType && genericResultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = genericResultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result).GetMethod(nameof(Result.FailureOf))!.MakeGenericMethod(valueType);
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException("Unsupported TResponse type in ValidationBehavior");
    }

}
