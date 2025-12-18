using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Refresh;

public class RefreshUserTokenCommand :IRequest<Result<TokensDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
