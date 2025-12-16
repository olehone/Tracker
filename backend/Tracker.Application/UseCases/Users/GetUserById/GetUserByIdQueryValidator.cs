using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Users.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{

    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }

}
