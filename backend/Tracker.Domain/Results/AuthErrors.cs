namespace Tracker.Domain.Results;

public static class AuthErrors
{
    public static readonly string RegistrationCode = "Auth.Registration";
    public static readonly string LoginCode = "Auth.Login";
    public static readonly string RefreshTokenCode = "Auth.RefreshToken";
    public static readonly string CurrentUserCode = "Auth.CurrentUser";

    public static readonly Error EmailExists = new(
        RegistrationCode,
        ErrorType.Conflict,
        "Email is already in use");

    public static readonly Error UsernameExists = new(
        RegistrationCode,
        ErrorType.Conflict,
        "Username is already in use");

    public static readonly Error UsernameOrEmailExists = new(
        RegistrationCode,
        ErrorType.Conflict,
        "Sorry, email or username was taken by someone else, try again");

    public static readonly Error PasswordIsIncorrect = Error.Validation("Password is incorrect");

    public static readonly Error RefreshTokenNotFound = new(
        RefreshTokenCode,
        ErrorType.NotFound,
        "Refresh token is not found");

    public static readonly Error RefreshTokenExpired = new(
        RefreshTokenCode,
        ErrorType.Unauthenticated,
        "Refresh token is expired");

    public static readonly Error Unauthenticated = new(
        CurrentUserCode,
        ErrorType.Unauthenticated,
        "You are not logged in");


    public static Error Forbidden(string? reason = null) =>  new(
        CurrentUserCode,
        ErrorType.Forbidden,
        reason ??"You do not have permission to perform this action");


}
