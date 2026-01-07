using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetSettings;

public class GetWorkspaceSettingsQuery : IRequest<Result<WorkspaceSettingsDto>>
{
    public required Guid WorkspaceId { get; set; }
}