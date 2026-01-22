namespace Tracker.Domain.Results;

public static class BoardErrors
{
    public static readonly Error UserNotInBoard = new(
        "BoardUser.NotInBoard",
        ErrorType.Conflict,
        "This user is not in the board"
    );

    public static readonly Error UserNotAssigned = new(
        "BoardItem.UserNotAssigned",
        ErrorType.Conflict,
        "This user is not assigned to the item"
    );
}
