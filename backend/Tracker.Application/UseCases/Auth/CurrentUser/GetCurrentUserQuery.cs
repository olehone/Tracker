using MediatR;
using Tracker.Domain.DTOs;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.CurrentUser;

public class GetCurrentUserQuery: IRequest<Result<UserDto>>
{
}

