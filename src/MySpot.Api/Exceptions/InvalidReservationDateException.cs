namespace MySpot.Api.Exceptions;

public sealed class InvalidReservationDateException : CustomException
{
    public InvalidReservationDateException(DateTime date)
        : base($"Reservation date: {date} is invalid")
    {
    }
}