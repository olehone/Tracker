using FluentValidation;

namespace Tracker.Application.UseCases;

public class PaginatedSearchValidator<T> 
    : AbstractValidator<T> where T : PaginatedSearch
{

    public PaginatedSearchValidator()
    {
        RuleFor(x => x.AmountInPage)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThan(0);

    }
}
