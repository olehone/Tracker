using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.SearchByUsername;

public sealed class GetUsersQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;
        IReadOnlyList<User> users;
        if (request.SearchQuery is null)
        {
            users = await uow.UserRepository.GetAllAsync(skip, request.AmountInPage);
        }
        else
        {
            users = await uow.UserRepository.SearchByUsernamePartAsync(
                request.SearchQuery, skip, request.AmountInPage);
        }
        return users.Select(user => user.ToDto()).ToList();
    }
}
