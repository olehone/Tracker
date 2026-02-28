using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Tracker.Domain.Options;
namespace Tracker.API.Controllers;

[Route("api/webhooks/stripe")]
[ApiController]
[AllowAnonymous]
public class StripeController(IMediator mediator, IOptions<StripeOptions> options) : ControllerBase
{
    private string EndpointSecret => options.Value.WebHookSecret;

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        Console.WriteLine("Hit webhook!");
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, EndpointSecret);
            Console.WriteLine("Event: {0}", stripeEvent.Data.ToString());

            // Handle the event
            // If on SDK version < 46, use class Events instead of EventTypes
            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Then define and call a method to handle the successful payment intent.
                // handlePaymentIntentSucceeded(paymentIntent);
            }
            else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                // Then define and call a method to handle the successful attachment of a PaymentMethod.
                // handlePaymentMethodAttached(paymentMethod);
            }
            // ... handle other event types
            else
            {
                // Unexpected event type
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.Message);
            return BadRequest();
        }
    }
}