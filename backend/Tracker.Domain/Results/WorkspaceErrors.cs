namespace Tracker.Domain.Results;

public static class WorkspaceErrors
{
    public static readonly Error UserNotInWorkspace = new(
        "WorkspaceUser.NotInWorkspace",
        ErrorType.Conflict,
        "This user is not in the workspace"
    );
}
