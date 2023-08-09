using MySpot.Application.Abstractions;

namespace MySpot.Application.Commands;

public record ChangeReservationLicencePlate(Guid ReservationId, string LicencePlate): ICommand;