using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tracker.API.Controllers;

[Route("api/health")]
[ApiController]
public class HeathController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AnonymousGetAsync()
    {
        return Ok();
    }

    [HttpGet("auth")]
    [Authorize]
    public async Task<IActionResult> AuthorizedGetAsync()
    {
        return Ok();
    }
}
