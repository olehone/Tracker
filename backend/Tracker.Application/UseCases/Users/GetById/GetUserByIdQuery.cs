using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Users.GetById;

public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public required Guid Id { get; set; }
}
