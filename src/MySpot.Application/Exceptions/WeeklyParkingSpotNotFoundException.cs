using MySpot.Core.Exceptions;

namespace MySpot.Application.Exceptions;

public sealed class WeeklyParkingSpotNotFoundException : CustomException
{
    public Guid Id { get; }

    public WeeklyParkingSpotNotFoundException(Guid id) 
        : base("Weekly parking spot with ID: {id} wasn't found")
    {
        Id = id;
    }
}

public sealed class ReservationNotFoundException : CustomException
{
    public Guid Id { get; }

    public ReservationNotFoundException(Guid id) 
        : base("Reservation with ID: {id} wasn't found")
    {
        Id = id;
    }
}