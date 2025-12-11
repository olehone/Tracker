namespace Tracker.Domain.Options;

public class RegistrationOptions
{
    public int PasswordMinimumLength { get; init; }
    public int UsernameMinimumLength { get; init; }
    public int UsernameMaximumLength { get; init; }
}
