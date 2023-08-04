using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Services;

namespace MySpot.Api.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationsService _reservationsService;
    
    public ReservationsController(IReservationsService reservationsService)
    {
        _reservationsService = reservationsService;
    }

    [HttpGet]
    public async Task<ActionResult<ReservationDto[]>> Get()
        => Ok(await _reservationsService.GetAllWeeklyAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReservationDto>> Get(Guid id)
    {
        var reservation = await _reservationsService.GetAsync(id);

        if (reservation is null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateReservation command)
    {
        var id = await _reservationsService.CreateAsync(command with { ReservationId = Guid.NewGuid() });
        if (id is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new {Id = id}, default);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Put(Guid id, ChangeReservationLicencePlate command)
    {
        var succeeded = await _reservationsService.UpdateAsync(command with { ReservationId = id });
        if (!succeeded)
        {
            return BadRequest();
        }
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var succeeded = await _reservationsService.DeleteAsync(new DeleteReservation(id));
        if (!succeeded)
        {
            return BadRequest();
        }
        
        return NoContent();
    }
}