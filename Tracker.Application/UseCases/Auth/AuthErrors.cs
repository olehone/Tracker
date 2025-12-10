using Tracker.Application.Results;

namespace Tracker.Application.UseCases.Auth;

public static class AuthErrors
{
    public static readonly string RegistrationCode = "Auth.Registration";
    public static readonly string LoginCode = "Auth.Login";

    public static readonly Error EmailExists = new(
        RegistrationCode,
        ErrorType.UniqueViolation,
        "Email is already in use");

    public static readonly Error UsernameExists = new(
        RegistrationCode,
        ErrorType.UniqueViolation,
        "Username is already in use");

    public static readonly Error UsernameOrEmailExists = new(
        RegistrationCode,
        ErrorType.Conflict,
        "Sorry, email or username was taken by someone else, try again");

    public static readonly Error UserNotFound = new(
        LoginCode,
        ErrorType.NotFound,
        "User is not found");

    public static readonly Error PasswordIsIncorrect = new(
        LoginCode,
        ErrorType.Validation,
        "Password is incorrect");
}
