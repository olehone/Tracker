using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.Update;

public class UpdateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
}