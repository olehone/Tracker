using Tracker.Domain.DTOs;
using MediatR;

namespace Tracker.Application.UseCases.Users;

public class RegisterUserCommand : IRequest<UserDto>
{
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
    public string Username {get; set;} = string.Empty;
    public string FirstName {get; set;} = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
