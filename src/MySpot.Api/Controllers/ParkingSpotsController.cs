using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;

namespace MySpot.Api.Controllers;

[ApiController]
[Route("parking-spots")]
public class ParkingSpotsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeeklyParkingSpotDto>>> Get(
        [FromQuery] GetWeeklyParkingSpots query,
        [FromServices] IQueryHandler<GetWeeklyParkingSpots, IEnumerable<WeeklyParkingSpotDto>> handler
        )
        => Ok(await handler.ExecuteAsync(query));

    [HttpPost("{parkingSpotId:guid}/reservations/vehicle")]
    public async Task<ActionResult> Post(Guid parkingSpotId, ReserveParkingSpotForVehicle command,
        [FromServices] ICommandHandler<ReserveParkingSpotForVehicle> hander)
    {
        await hander.HandleAsync(command with { ReservationId = parkingSpotId });
        return NoContent();
    }
    
    [HttpPost("reservations/cleaning")]
    public async Task<ActionResult> Post(Guid parkingSpotId, ReserveParkingSpotForCleaning command,
        [FromServices] ICommandHandler<ReserveParkingSpotForCleaning> hander)
    {
        await hander.HandleAsync(command);
        return NoContent();
    }
    
    [HttpPut("reservations/{reservationId:guid}")]
    public async Task<ActionResult> Put(Guid reservationId, ChangeReservationLicencePlate command,
        [FromServices] ICommandHandler<ChangeReservationLicencePlate> handler)
    {
        await handler.HandleAsync(command with {ReservationId = reservationId});
        return NoContent();
    }

    [HttpDelete("reservations/{reservationId:guid}")]
    public async Task<ActionResult> Delete(Guid reservationId, [FromServices] ICommandHandler<DeleteReservation> handler)
    {
        await handler.HandleAsync(new DeleteReservation(reservationId));
        return NoContent();
    }
    
    
    
}