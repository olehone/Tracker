namespace Tracker.Domain.Options;

public class LoginOptions
{
    public const string SectionName = "LoginOptions";
    public int PasswordMinimumLength { get; init; }
}
