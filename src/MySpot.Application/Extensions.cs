using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Services;

[assembly:InternalsVisibleTo("MySpot.Tests.Unit")]
namespace MySpot.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IReservationsService, ReservationsService>();

        return services;
    }
}