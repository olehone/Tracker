using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(query.Id);

        return user is null
            ? Error.NotFound("User")
            : user.ToDto();
    }
}
