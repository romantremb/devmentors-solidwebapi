using MySpot.Application.Abstractions;
using MySpot.Application.Exceptions;
using MySpot.Core.Abstractions;
using MySpot.Core.DomainServices;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Application.Commands.Handlers;

internal sealed class ReserveParkingSpotForVehicleHandler: ICommandHandler<ReserveParkingSpotForVehicle>
{
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
    private readonly IUserRepository _userRepository;
    private readonly IParkingReservationService _reservationService;
    private readonly IClock _clock;

    public ReserveParkingSpotForVehicleHandler(
        IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
        IUserRepository userRepository,
        IParkingReservationService reservationService,
        IClock clock)
    {
        _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        _userRepository = userRepository;
        _reservationService = reservationService;
        _clock = clock;
    }
    
    public async Task HandleAsync(ReserveParkingSpotForVehicle command)
    {
        var (id, reservationId, userId, licencePlate, capacity, date) = command;
        
        var parkingSpotId = new ParkingSpotId(id);
        var week = new Week(_clock.Current());
        var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
        
        if (parkingSpotToReserve is null)
        {
            throw new WeeklyParkingSpotNotFoundException(parkingSpotId);
        }

        var user = await _userRepository.GetById(userId);
        if (user is null)
        {
            throw new UserNotFoundException(user.Id);
        }
        
        var reservation = new VehicleReservation(reservationId, userId, new EmployeeName(user.FullName), licencePlate, capacity, new Date(date));
        
        _reservationService.ReserveSpotForVehicle(weeklyParkingSpots,  JobTitle.Employee, 
            parkingSpotToReserve, reservation);
        
        await _weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);
    }
}