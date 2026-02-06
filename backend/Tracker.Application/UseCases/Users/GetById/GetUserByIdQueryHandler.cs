using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetById;

public sealed class GetUserByIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        return user.ToDto();
    }
}
