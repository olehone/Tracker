using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Refresh;

public class RefreshUserTokenCommand :IRequest<Result<TokensDto>>
{
    public required string RefreshToken { get; set; }
}
