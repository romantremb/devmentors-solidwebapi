using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;

namespace MySpot.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> SignUp(SignUp command, [FromServices] ICommandHandler<SignUp> handler)
    {
        await handler.HandleAsync(command with { UserId = Guid.NewGuid() });
        return NoContent();
    }
    
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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] GetUsers query, 
        [FromServices] IQueryHandler<GetUsers, IEnumerable<UserDto>> handler)
        => Ok(await handler.HandleAsync(query));
}