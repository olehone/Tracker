using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.UserSubscriptions.CreateSession;

public class CreateCheckoutSessionCommand : IRequest<Result<string>>
{
    public required SubscriptionPlan Plan { get; set; }
}