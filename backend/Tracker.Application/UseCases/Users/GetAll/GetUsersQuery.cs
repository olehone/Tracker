using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetAll;

public class GetUsersQuery
    :PaginatedSearch, IRequest<Result<Paginated<UserDto>>>
{
}
