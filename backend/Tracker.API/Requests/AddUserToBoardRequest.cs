using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class AddUserToBoardRequest 
{
    public required Guid BoardId { get; set; }
    public required Guid UserId { get; set; }
    public required UserBoardRole Role { get; set; }
}