using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Users.Current;
using Tracker.Application.UseCases.Users.DeleteAvatar;
using Tracker.Application.UseCases.Users.GetAll;
using Tracker.Application.UseCases.Users.GetAvatarUrl;
using Tracker.Application.UseCases.Users.GetById;
using Tracker.Application.UseCases.Users.GetCurrentPermissions;
using Tracker.Application.UseCases.Users.Update;
using Tracker.Application.UseCases.Users.UploadAvatar;
using Tracker.Application.UseCases.Workspaces.GetAllForUser;
using Tracker.Application.UseCases.Workspaces.GetMutual;

namespace Tracker.API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var mediatorRequest = new GetUserByIdQuery { Id = id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var mediatorRequest = new UpdateUserCommand
        {
            UserId = id,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentAsync()
    {
        var response = await mediator.Send(new GetCurrentUserQuery());
        return response.ToActionResult();
    }

    [HttpGet("me/permissions")]
    public async Task<IActionResult> GetCurrentUserPermissionsAsync()
    {
        var response = await mediator.Send(new GetCurrentUserPermissionsQuery());
        return response.ToActionResult();
    }

    [Authorize(Roles = "Admin,Owner")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAsync([FromQuery] PaginatedSearchRequest request)
    {
        var mediatorRequest = new GetUsersQuery
        {
            SearchQuery = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("{id:guid}/workspaces")]
    public async Task<IActionResult> GetMutualWorkspacesAsync(Guid id,
        [FromQuery] PaginatedSearchRequest request)
    {
        var mediatorRequest = new GetMutualWorkspacesQuery
        {
            TargetUserId = id,
            SearchQuery = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("{id:guid}/workspaces/all")]
    public async Task<IActionResult> GetAllWorkspacesAsync(
        Guid id,
        [FromQuery] PaginatedSearchRequest request)
    {
        var mediatorRequest = new GetAllWorkspacesByUserQuery
        {
            Id = id,
            SearchQuery = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("{id:guid}/avatar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvatarUrlAsync(Guid id, [FromQuery] long version)
    {
        var mediatorRequest = new GetAvatarUrlCommand
        {
            UserId = id,
            Version = version
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsFailure)
        {
            return response.ToActionResult();
        }

        return Redirect(response.Value);
    }

    [HttpPost("{id:guid}/avatar")]
    public async Task<IActionResult> UploadAvatarAsync(Guid id,
        [FromForm] FileUploadRequest request)
    {
        await using var stream = request.File.OpenReadStream();
        var mediatorRequest = new UploadAvatarCommand
        {
            UserId = id,
            Content = stream,
            ContentType = request.File.ContentType,
            ContentLength = request.File.Length
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{id:guid}/avatar")]
    public async Task<IActionResult> DeleteAvatarAsync(Guid id)
    {
        var mediatorRequest = new DeleteAvatarCommand
        {
            UserId = id,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
