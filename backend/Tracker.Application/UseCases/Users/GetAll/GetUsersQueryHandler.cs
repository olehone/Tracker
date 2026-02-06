using MediatR;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetAll;

public sealed class GetUsersQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUsersQuery, Result<Paginated<UserDto>>>
{
    public async Task<Result<Paginated<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {

        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;
        var count = await uow.UserRepository
            .CountAsync(request.SearchQuery);
        if (count == 0)
        {
            return Paginated<UserDto>.Empty();
        }

        var users = await uow.UserRepository
            .GetAsync(request.SearchQuery, skip, request.AmountInPage);

        var userDtos = users.Select(user => user.ToDto())
            .ToList();

        return new Paginated<UserDto>
        {
            Items = userDtos,
            TotalCount = count
        };
    }
}
