namespace MySpot.Api.Exceptions;

public class ParkingSpotAlreadyReservedException : CustomException
{
    public ParkingSpotAlreadyReservedException(string parkingSpotName, DateTime date) 
        : base($"Parking spot with name {parkingSpotName} is already reserved for date: {date}")
    {
    }
}