using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.SearchByUsername;

public class GetUsersQuery()
    :PaginatedSearch, IRequest<Result<List<UserDto>>>
{
}
