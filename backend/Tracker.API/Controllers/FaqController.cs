using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Faq.Ask;
using Tracker.Application.UseCases.Faq.Seed;

namespace Tracker.API.Controllers;

[Route("api/faq")]
[ApiController]
[Authorize]
public class FaqController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin,Owner")]
    [HttpPost("admin/seed")]
    public async Task<IActionResult> SeedAsync()
    {
        var result = await mediator.Send(new SeedFaqCommand());
        return result.ToActionResult();
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskAsync([FromBody] FaqRequest request)
    {
        var mediatorRequest = new AskFaqQuestionCommand
        {
            Question = request.Question
        };
        var result = await mediator.Send(mediatorRequest);
        return result.ToActionResult();
    }
}