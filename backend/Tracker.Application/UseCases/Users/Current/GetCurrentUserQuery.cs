using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.Current;

public class GetCurrentUserQuery: IRequest<Result<UserDto>>
{
}