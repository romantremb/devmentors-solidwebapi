using MySpot.Application.Commands;
using MySpot.Application.Services;
using MySpot.Core.Abstractions;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Tests.Unit.Shared;
using Shouldly;

namespace MySpot.Tests.Unit.Services;

public class ReservationsServiceTests
{
    [Fact]
    public async Task given_valid_command_create_should_add_reservation()
    {
        var command = new ReserveParkingSpotForVehicle(Guid.Parse("00000000-0000-0000-0000-000000000001"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", _clock.Current().AddDays(1));

        await _service.ReserveForVehicleAsync(command);

        reservationId.ShouldNotBeNull();
        reservationId.Value.ShouldBe(command.ReservationId);
    }
    
    [Fact]
    public async Task given_invalid_parking_spot_id_create_should_fail()
    {
        var command = new ReserveParkingSpotForVehicle(Guid.Parse("00000000-0000-0000-0000-0000000000A1"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", DateTime.UtcNow.AddDays(1));

        var reservationId = await _service.ReserveForVehicleAsync(command);

        reservationId.ShouldBeNull();
    }

    [Fact]
    public async Task given_reservation_for_already_taken_date_create_should_fail()
    {
        var command = new ReserveParkingSpotForVehicle(Guid.Parse("00000000-0000-0000-0000-000000000001"), 
            Guid.NewGuid(), "Joe Doe", "XYZ123", DateTime.UtcNow.AddDays(1));
        _service.ReserveForVehicleAsync(command);
        
        var reservationId = await _service.ReserveForVehicleAsync(command);

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