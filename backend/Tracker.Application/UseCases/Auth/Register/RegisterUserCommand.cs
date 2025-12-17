using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Register;

public class RegisterUserCommand : IRequest<Result<AuthResponse>>
{
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
    public string Username {get; set;} = string.Empty;
    public string FirstName {get; set;} = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
