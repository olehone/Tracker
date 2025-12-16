using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Domain.DTOs;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    :IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(query.Id);

        if (user == null)
        {
            return new Error(
                "User.NotFound",
                ErrorType.NotFound,
                "User with this id is not found");
        }
        return user.ToDto();
    }
}
