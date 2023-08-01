using MySpot.Core.Entities;
using MySpot.Core.Exceptions;
using MySpot.Core.ValueObjects;
using Shouldly;

namespace MySpot.Tests.Unit.Entities;

public class WeeklyParkingSpotTests
{
    [Theory]
    [InlineData("2020-02-02")]
    [InlineData("2025-02-02")]
    [InlineData("2020-02-24")]
    public void given_invalid_date_add_reservation_should_fail(string dateString)
    {
        var invalidDate = DateTime.Parse(dateString);
        
        //ARRANGE
        var reservation = new Reservation(Guid.NewGuid(), "Joe Doe", "XYZ123", 
            new Date(invalidDate));

        //ACT
        var exception = Record.Exception(() => _weeklyParkingSpot.AddReservation(reservation, _now));

        //ASSERT
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidReservationDateException>();
    }

    [Fact]
    public void given_reservation_for_already_existing_date_add_reservation_should_fail()
    {
        //ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new Reservation(Guid.NewGuid(), "Joe Doe", "XYZ123", reservationDate);
        _weeklyParkingSpot.AddReservation(reservation, reservationDate);
        
        //ACT
        var exception = Record.Exception(() => _weeklyParkingSpot.AddReservation(reservation, reservationDate));
        
        //ASSERT
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ParkingSpotAlreadyReservedException>();
    }
    
    [Fact]
    public void given_reservation_for_not_taken_date_add_reservation_should_succeed()
    {
        //ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new Reservation(Guid.NewGuid(), "Joe Doe", "XYZ123", reservationDate);
        
        //ACT
        _weeklyParkingSpot.AddReservation(reservation, reservationDate);
        
        //ASSERT
        _weeklyParkingSpot.Reservations.ShouldContain(reservation);
        _weeklyParkingSpot.Reservations.ShouldHaveSingleItem();
    }

    private readonly WeeklyParkingSpot _weeklyParkingSpot;
    private readonly Date _now;
    
    public WeeklyParkingSpotTests()
    {
        _now =  new Date(DateTime.Parse("2022-02-25"));
        _weeklyParkingSpot = new WeeklyParkingSpot(Guid.NewGuid(), new Week(_now), "P1");
    }
}