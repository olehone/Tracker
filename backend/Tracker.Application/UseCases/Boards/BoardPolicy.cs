using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Boards;

public static class BoardPolicy
{
    public static bool IsActionAllowed(
        GlobalRole globalRole,
        UserWorkspace workspaceMembership,
        UserBoard boardMembership,
        WorkspaceSettings workspaceSettings,
        BoardSettings boardSettings,
        BoardAction boardAction)
    {
        return true;
    }
}