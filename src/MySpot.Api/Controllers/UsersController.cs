using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;
using MySpot.Application.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace MySpot.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [Authorize(Policy = "is-admin")]
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Get(Guid userId, [FromServices] IQueryHandler<GetUser, UserDto> handler)
    {
        var user = await handler.HandleAsync(new GetUser {UserId = userId});
        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet]
    [Authorize(Policy = "is-admin")]
    [SwaggerOperation("Get list of all the users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] GetUsers query, 
        [FromServices] IQueryHandler<GetUsers, IEnumerable<UserDto>> handler)
        => Ok(await handler.HandleAsync(query));

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> Get([FromServices] IQueryHandler<GetUser, UserDto> handler)
    {
        if (string.IsNullOrEmpty(HttpContext.User.Identity.Name))
        {
            return NotFound();
        }

        var userId = Guid.Parse(HttpContext.User.Identity?.Name);
        var user = await handler.HandleAsync(new GetUser {UserId = userId});
        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    [AllowAnonymous]
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SignUp(SignUp command, [FromServices] ICommandHandler<SignUp> handler)
    {
        await handler.HandleAsync(command with { UserId = Guid.NewGuid() });
        return CreatedAtAction(nameof(Get), new {command.UserId}, null);
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("Sign in the user and return the json web token")]
    public async Task<ActionResult<JwtDto>> SignIn(SignIn command, 
        [FromServices] ICommandHandler<SignIn> handler, 
        [FromServices] ITokenStorage tokenStorage)
    {
        await handler.HandleAsync(command);
        return tokenStorage.Get();
    }
    
}