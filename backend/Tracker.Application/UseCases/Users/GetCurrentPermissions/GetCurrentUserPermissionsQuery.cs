using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetCurrentPermissions;

public class GetCurrentUserPermissionsQuery : IRequest<Result<UserPermissionsDto>>
{
}
