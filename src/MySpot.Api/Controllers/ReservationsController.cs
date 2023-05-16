using Microsoft.AspNetCore.Mvc;
using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.Services;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private static readonly ReservationsService _service = new(new Clock(), new WeeklyParkingSpot[]
    {
        new(Guid.Parse("00000000-0000-0000-0000-000000000001"), new Week(new Clock().Current()), "P1"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000002"), new Week(new Clock().Current()), "P2"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000003"), new Week(new Clock().Current()), "P3"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000004"), new Week(new Clock().Current()), "P4"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000005"), new Week(new Clock().Current()), "P5")
    });

    [HttpGet]
    public ActionResult<ReservationDto[]> Get()
        => Ok(_service.GetAllWeekly());

    [HttpGet("{id:guid}")]
    public ActionResult<ReservationDto> Get(Guid id)
    {
        var reservation = _service.Get(id);

        if (reservation is null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

    [HttpPost]
    public ActionResult Post(CreateReservation command)
    {
        var id = _service.Create(command with { ReservationId = Guid.NewGuid() });
        if (id is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new {Id = id}, default);
    }

    [HttpPut("{id:guid}")]
    public ActionResult Put(Guid id, ChangeReservationLicencePlate command)
    {
        var succeeded = _service.Update(command with { ReservationId = id });
        if (!succeeded)
        {
            return BadRequest();
        }
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        var succeeded = _service.Delete(new DeleteReservation(id));
        if (!succeeded)
        {
            return BadRequest();
        }
        
        return NoContent();
    }
}