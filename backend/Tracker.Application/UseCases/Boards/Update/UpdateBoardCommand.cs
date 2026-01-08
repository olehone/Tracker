using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Boards.Update;

public class UpdateBoardCommand : IRequest<Result>
{
    public Guid BoardId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required BoardVisibility Visibility { get; set; }
    public required BoardPermissionRoles PermissionRoles { get; set; }
}