using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.SearchUsersByUsername;

public sealed class SearchUsersByUsernamePartQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    :IRequestHandler<SearchUsersByUsernamePartQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(
        SearchUsersByUsernamePartQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        int skip = (request.Page - 1) * request.AmountInPage;
        var users = await uow.UserRepository.SearchByUsernamePartAsync(request.Username,
                                                                       skip,
                                                                       request.AmountInPage);
        return users.Select(user => user.ToDto()).ToList();
    }
}
