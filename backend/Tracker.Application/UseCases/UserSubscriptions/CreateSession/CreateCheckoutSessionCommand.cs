using MediatR;

namespace Tracker.Application.UseCases.UserSubscriptions.CreateSession;

public class CreateCheckoutSessionCommand : IRequest<string>
{
    public required Guid UserId { get; set; }
    public required string PriceId { get; set; }
}