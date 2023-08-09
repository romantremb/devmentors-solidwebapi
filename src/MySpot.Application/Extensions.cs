using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Abstractions;

[assembly:InternalsVisibleTo("MySpot.Tests.Unit")]
namespace MySpot.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommand).Assembly;
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }
}