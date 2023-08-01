using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.Exceptions;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Services;

public sealed class ReservationsService : IReservationsService
{
    private readonly IClock _clock;

    private readonly IEnumerable<WeeklyParkingSpot> _weeklyParkingSpots;

    public ReservationsService(IClock clock, IEnumerable<WeeklyParkingSpot> weeklyParkingSpots)
    {
        _clock = clock;
        _weeklyParkingSpots = weeklyParkingSpots;
    }

    public IEnumerable<ReservationDto> GetAllWeekly() 
        => _weeklyParkingSpots
            .SelectMany(x => x.Reservations)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                EmployeeName = r.EmployeeName,
                Date = r.Date.Value.Date
            });

    public ReservationDto Get(Guid id)
        => GetAllWeekly().SingleOrDefault(x => x.Id == id);

    public Guid? Create(CreateReservation command)
    {
        try
        {
            var (id, reservationId, employeeName, licencePlate, date) = command;
        
            var parkingSpotId = new ParkingSpotId(id);
            var weeklyParkingSpot = _weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
        
            if (weeklyParkingSpot is null)
            {
                return default;
            }
        
            var reservation = new Reservation(reservationId, employeeName, licencePlate, new Date(date));
        
            weeklyParkingSpot.AddReservation(reservation, new Date(CurrentDate()));
            return reservation.Id;
        }
        catch (CustomException)
        {
            return default;
        }
    }

    public bool Update(ChangeReservationLicencePlate command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
        {
            return false;
        }

        var reservationId = new ReservationId(command.ReservationId);
        var reservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id == reservationId);
        if (reservation is null)
        {
            return false;
        }

        reservation.ChangeLicencePlate(command.LicencePlate);
        return true;
    }

    public bool Delete(DeleteReservation command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);

        if (weeklyParkingSpot is null)
        {
            return false;
        }

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        return true;
    }

    private WeeklyParkingSpot GetWeeklyParkingSpotByReservation(ReservationId id)
        => _weeklyParkingSpots
            .SingleOrDefault(x => x.Reservations.Any(r => r.Id == id));

    private DateTime CurrentDate() => _clock.Current();
}