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

    [HttpPost("vehicle")]
    public async Task<ActionResult> Post(ReserveParkingSpotForVehicle command)
    {
        await _reservationsService.ReserveForVehicleAsync(command with { ReservationId = Guid.NewGuid() });
        return CreatedAtAction(nameof(Get), new {Id = command.ReservationId}, default);
    }
    
    [HttpPost("cleaning")]
    public async Task<ActionResult> Post(ReserveParkingSpotForCleaning command)
    {
        await _reservationsService.ReserveForCleaningAsync(command);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Put(Guid id, ChangeReservationLicencePlate command)
    {
        await _reservationsService.ChangeReservationLicencePlateAsync(command with { ReservationId = id });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _reservationsService.DeleteAsync(new DeleteReservation(id));
        
        return NoContent();
    }
}