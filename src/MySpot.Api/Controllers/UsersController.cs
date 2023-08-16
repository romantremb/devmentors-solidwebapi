using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;
using MySpot.Application.Security;

namespace MySpot.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get(Guid userId, [FromServices] IQueryHandler<GetUser, UserDto> handler)
    {
        var user = await handler.HandleAsync(new GetUser {UserId = userId});
        if (user is null)
        {
            return NotFound();
        }

        return user;
    }
    
    [HttpGet("me")]
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] GetUsers query, 
        [FromServices] IQueryHandler<GetUsers, IEnumerable<UserDto>> handler)
        => Ok(await handler.HandleAsync(query));

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> SignUp(SignUp command, [FromServices] ICommandHandler<SignUp> handler)
    {
        await handler.HandleAsync(command with { UserId = Guid.NewGuid() });
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SignIn(SignIn command, 
        [FromServices] ICommandHandler<SignIn> handler, 
        [FromServices] ITokenStorage tokenStorage)
    {
        await handler.HandleAsync(command);
        return tokenStorage.Get();
    }
    
}