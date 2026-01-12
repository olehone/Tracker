namespace Tracker.API.Requests;

public class RemoveUserFromBoardRequest
{
    public required Guid BoardId { get; set; }
    public required Guid UserId { get; set; }
}