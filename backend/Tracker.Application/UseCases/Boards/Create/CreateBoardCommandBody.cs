namespace Tracker.Application.UseCases.Boards.Create;

public sealed record class CreateBoardCommandBody(string Title, string? Description);
