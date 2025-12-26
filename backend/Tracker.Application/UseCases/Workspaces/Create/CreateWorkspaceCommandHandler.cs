using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Register;

public sealed class CreateWorkspaceCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceDto>>
{
    public async Task<Result<WorkspaceDto>> Handle(
        CreateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = new Workspace()
        {
            Title = request.Title,
            Description = request.Description,
        };
        await uow.WorkspaceRepository.AddAsync(workspace);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : workspace.ToDto();
    }
}
