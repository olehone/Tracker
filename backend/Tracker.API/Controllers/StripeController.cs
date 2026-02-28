using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Tracker.Application.UseCases.UserSubscriptions.Activate;
using Tracker.Application.UseCases.UserSubscriptions.Cancel;
using Tracker.Application.UseCases.UserSubscriptions.Update;
using Tracker.Domain.Enums;
using Tracker.Domain.Options;

namespace Tracker.API.Controllers;

[Route("api/webhooks/stripe")]
[ApiController]
[AllowAnonymous]
public class StripeWebhookController(
    IMediator mediator,
    IOptions<StripeOptions> options,
    ILogger<StripeWebhookController> logger
) : ControllerBase
{
    private string WebHookSecret => options.Value.WebHookSecret;
    private string BasicSubscription => options.Value.BasicSubscriptionName;
    private string ProSubscription => options.Value.ProSubscriptionName;

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var signatureHeader = Request.Headers["Stripe-Signature"];
            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, WebHookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    await OnCheckoutSessionCompleted(stripeEvent);
                    break;

                case EventTypes.CustomerSubscriptionUpdated:
                    await OnCustomerSubscriptionUpdated(stripeEvent);
                    break;

                case EventTypes.CustomerSubscriptionDeleted:
                    await OnCustomerSubscriptionDeleted(stripeEvent);
                    break;

                default:
                    logger.LogWarning("Stripe event with {type} type is not handled", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }

    private async Task<IActionResult> OnCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Session;
        var subService = new SubscriptionService();
        var stripeSub = await subService.GetAsync(session!.SubscriptionId);

        var plan = MapToPlan(stripeSub.Items.Data[0].Price.Id);
        if (plan is null)
        {
            return BadRequest();
        }

        var mediatorRequest = new ActivateUserSubscriptionCommand
        {
            UserId = Guid.Parse(session.Metadata["userId"]),
            StripeCustomerId = session.CustomerId,
            StripeSubscriptionId = session.SubscriptionId,
            Plan = plan.Value,
            CurrentPeriodEnd = stripeSub.Items.Data[0].CurrentPeriodEnd
        };

        await mediator.Send(mediatorRequest);
        return Ok();
    }

    private async Task<IActionResult> OnCustomerSubscriptionUpdated(Event stripeEvent)
    {
        var updated = stripeEvent.Data.Object as Subscription;

        var plan = MapToPlan(updated!.Items.Data[0].Price.Id);
        if (plan is null)
        {
            return BadRequest();
        }

        var mediatorRequest = new UpdateUserSubscriptionCommand
        {
            StripeSubscriptionId = updated!.Id,
            Plan = plan.Value,
            CurrentPeriodEnd = updated.Items.Data[0].CurrentPeriodEnd
        };
        await mediator.Send(mediatorRequest);
        return Ok();
    }

    private async Task OnCustomerSubscriptionDeleted(Event stripeEvent)
    {
        var deleted = stripeEvent.Data.Object as Subscription;
        var mediatorRequest = new CancelUserSubscriptionCommand
        {
            StripeSubscriptionId = deleted!.Id
        };
        await mediator.Send(mediatorRequest);
    }

    private SubscriptionPlan? MapToPlan(string priceId)
    {
        if (priceId == BasicSubscription)
        {
            return SubscriptionPlan.Basic;
        }

        if (priceId == ProSubscription)
        {
            return SubscriptionPlan.Pro;
        }

        logger.LogError("Stripe sent unknown priceId ({priceId}), cannot map it", priceId);
        return null;
    }
}