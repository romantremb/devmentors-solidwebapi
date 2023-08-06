using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Exceptions;
using MySpot.Core.Abstractions;
using MySpot.Core.DomainServices;
using MySpot.Core.Entities;
using MySpot.Core.Exceptions;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Application.Services;

internal sealed class ReservationsService : IReservationsService
{
    private readonly IClock _clock;
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
    private readonly IParkingReservationService _parkingReservationService;

    public ReservationsService(
        IClock clock, 
        IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
        IParkingReservationService parkingReservationService)
    {
        _clock = clock;
        _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        _parkingReservationService = parkingReservationService;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllWeeklyAsync() 
        => (await _weeklyParkingSpotRepository
            .GetAllAsync())
            .SelectMany(x => x.Reservations)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                EmployeeName = r.EmployeeName,
                Date = r.Date.Value.Date
            });

    public async Task<ReservationDto> GetAsync(Guid id)
        => (await GetAllWeeklyAsync()).SingleOrDefault(x => x.Id == id);

    public async Task CreateAsync(CreateReservation command)
    {
        var (id, reservationId, employeeName, licencePlate, date) = command;
        
        var parkingSpotId = new ParkingSpotId(id);
        var week = new Week(_clock.Current());
        var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
        
        if (parkingSpotToReserve is null)
        {
            throw new WeeklyParkingSpotNotFoundException(parkingSpotId);
        }
        
        var reservation = new Reservation(reservationId, employeeName, licencePlate, new Date(date));
        
        _parkingReservationService.ReserveSpotForVehicle(weeklyParkingSpots,  JobTitle.Employee, 
            parkingSpotToReserve, reservation);
        
        await _weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);
    }

    public async Task UpdateAsync(ChangeReservationLicencePlate command)
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
        {
            throw new WeeklyParkingSpotNotFoundException(command.ReservationId);
        }

        var reservationId = new ReservationId(command.ReservationId);
        var reservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id == reservationId);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(reservationId);
        }

        reservation.ChangeLicencePlate(command.LicencePlate);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
    }

    public async Task DeleteAsync(DeleteReservation command)
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservation(command.ReservationId);

        if (weeklyParkingSpot is null)
        {
            throw new WeeklyParkingSpotNotFoundException(command.ReservationId);
        }

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
    }

    private async Task<WeeklyParkingSpot> GetWeeklyParkingSpotByReservation(ReservationId id)
        => (await _weeklyParkingSpotRepository
            .GetAllAsync())
            .SingleOrDefault(x => x.Reservations.Any(r => r.Id == id));

    private DateTime CurrentDate() => _clock.Current();
}