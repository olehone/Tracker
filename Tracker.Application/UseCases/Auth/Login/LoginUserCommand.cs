using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Login;

public class LoginUserCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
