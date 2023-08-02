using MySpot.Application.Commands;
using MySpot.Application.Services;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Tests.Unit.Shared;
using Shouldly;

namespace MySpot.Tests.Unit.Services;

public class ReservationsServiceTests
{
    [Fact]
    public void given_valid_command_create_should_add_reservation()
    {
        var command = new CreateReservation(Guid.Parse("00000000-0000-0000-0000-000000000001"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", _clock.Current().AddDays(1));

        var reservationId = _service.Create(command);

        reservationId.ShouldNotBeNull();
        reservationId.Value.ShouldBe(command.ReservationId);
    }
    
    [Fact]
    public void given_invalid_parking_spot_id_create_should_fail()
    {
        var command = new CreateReservation(Guid.Parse("00000000-0000-0000-0000-0000000000A1"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", DateTime.UtcNow.AddDays(1));

        var reservationId = _service.Create(command);

        reservationId.ShouldBeNull();
    }

    [Fact]
    public void given_reservation_for_already_taken_date_create_should_fail()
    {
        var command = new CreateReservation(Guid.Parse("00000000-0000-0000-0000-000000000001"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", DateTime.UtcNow.AddDays(1));
        _service.Create(command);
        
        var reservationId = _service.Create(command);

        reservationId.ShouldBeNull();
    }
    
    private readonly ReservationsService _service;
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
    private readonly IClock _clock;
    
    public ReservationsServiceTests()
    {
        _clock = new TestClock();
        _weeklyParkingSpotRepository = new InMemoryWeeklyParkingSpotRepository(_clock);
        _service = new ReservationsService(_clock, _weeklyParkingSpotRepository);
    }
    
}