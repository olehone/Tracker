namespace Tracker.Domain.Options;

public class LoginOptions
{
    public const string SectionName = "LoginOptions";
    
    public required int PasswordMinimumLength { get; init; }
}
