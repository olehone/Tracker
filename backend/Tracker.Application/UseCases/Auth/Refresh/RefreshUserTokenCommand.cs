using MediatR;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Refresh;

public class RefreshUserTokenCommand :IRequest<Result<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
