using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.SearchByUsername;

public class SearchUsersByUsernamePartQuery() : IRequest<Result<List<UserDto>>>
{
    public required string Username { get; set; }
    public required int Page { get; set; } = 1;
    public required int AmountInPage { get; set; } = 20;
}
