using Tracker.Domain.DTOs;
using Tracker.Application.Results;
using MediatR;

namespace Tracker.Application.UseCases.Auth.Register;

public class RegisterUserCommand : IRequest<Result<UserDto>>
{
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
    public string Username {get; set;} = string.Empty;
    public string FirstName {get; set;} = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
