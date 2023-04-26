using MySpot.Api.Models;

namespace MySpot.Api.Services;

public sealed class ReservationsService
{
    private static string[] _parkingSpotNames = {"P1", "P2", "P3", "P4", "P5"};
    private static readonly List<Reservation> _reservations = new();

    private static int Id = 1;

    public IEnumerable<Reservation> GetAll() => _reservations;

    public Reservation Get(int id)
        => _reservations.SingleOrDefault(r => r.Id == id);

    public int? Create(Reservation reservation)
    {
        reservation.Id = Id;
        reservation.Date = DateTime.Now.AddDays(1).Date;

        if (_parkingSpotNames.All(x => x != reservation.ParkingSpotName))
        {
            return default;
        }

        var reservationAlreadyExists = _reservations.Any(x => x.Date.Date == reservation.Date.Date &&
                                                              x.ParkingSpotName == reservation.ParkingSpotName);
        if (reservationAlreadyExists)
        {
            return default;
        }

        Id++;
        _reservations.Add(reservation);
        return reservation.Id;
    }

    public bool Update(Reservation reservation)
    {
        var existingReservation = _reservations.SingleOrDefault(x => x.Id == reservation.Id);

        if (existingReservation is null)
        {
            return false;
        }

        existingReservation.LicencePlate = reservation.LicencePlate;
        return true;
    }

    public bool Delete(int id)
    {
        var existingReservation = _reservations.SingleOrDefault(x => x.Id == id);

        if (existingReservation is null)
        {
            return false;
        }

        _reservations.Remove(existingReservation);
        return true;
    }
}