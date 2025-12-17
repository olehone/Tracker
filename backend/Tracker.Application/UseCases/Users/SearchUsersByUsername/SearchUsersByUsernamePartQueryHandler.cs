using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.SearchUsersByUsername;

public sealed class SearchUsersByUsernamePartQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    :IRequestHandler<SearchUsersByUsernamePartQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(
        SearchUsersByUsernamePartQuery query,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        int skip = (query.Page - 1) * query.AmountInPage;
        return await uow.UserRepository.SearchUsersByUsernameAsync(
            query.Username,
            skip, 
            query.AmountInPage);
    }
}
