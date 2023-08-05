using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Services;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Time;

[assembly:InternalsVisibleTo("MySpot.Tests.Unit")]
namespace MySpot.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddPostgres(configuration);

        services.AddSingleton<ExceptionMiddleware>();
        services.AddSingleton<IClock, Clock>();
        
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.MapControllers();
        
        return app;
    } 

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();

        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);
        
        return options;
    }
}