namespace Tracker.Domain.Requests;

public class SearchUsersByUsernamePartRequest()
{
    public required string Username { get; set; }
    public required int Page { get; set; }
    public required int AmountInPage { get; set; }
}
