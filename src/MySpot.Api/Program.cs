using MySpot.Api.Entities;
using MySpot.Api.Services;
using MySpot.Api.ValueObjects;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IClock, Clock>()
    .AddSingleton<IReservationsService, ReservationsService>()
    .AddSingleton<IEnumerable<WeeklyParkingSpot>>(new WeeklyParkingSpot[]
    {
        new(Guid.Parse("00000000-0000-0000-0000-000000000001"), new Week(new Clock().Current()), "P1"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000002"), new Week(new Clock().Current()), "P2"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000003"), new Week(new Clock().Current()), "P3"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000004"), new Week(new Clock().Current()), "P4"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000005"), new Week(new Clock().Current()), "P5")
    })
    .AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
