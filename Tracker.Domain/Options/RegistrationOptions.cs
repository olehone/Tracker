namespace Tracker.Domain.Options;

public class RegistrationOptions
{
    public const string SectionName = "RegistrationOptions";
    public int PasswordMinimumLength { get; init; }
    public int UsernameMinimumLength { get; init; }
    public int UsernameMaximumLength { get; init; }
}
