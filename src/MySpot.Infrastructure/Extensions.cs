using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MySpot.Application.Abstractions;
using MySpot.Core.Abstractions;
using MySpot.Infrastructure.Auth;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Security;
using MySpot.Infrastructure.Time;

[assembly:InternalsVisibleTo("MySpot.Tests.Unit")]
[assembly:InternalsVisibleTo("MySpot.Tests.Integration")]
namespace MySpot.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddPostgres(configuration);

        services.AddSingleton<ExceptionMiddleware>();
        services.AddSingleton<IClock, Clock>();
        services.AddHttpContextAccessor();

        services.AddCustomLogging();
        services.AddSecurity();
        services.AddAuth(configuration);
        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "MySpotApi",
                Version = "v1"
            });
        });
        
        var infrastructureAssembly = typeof(ExceptionMiddleware).Assembly;
        services.Scan(s => s.FromAssemblies(infrastructureAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
        
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = "docs";
            reDoc.SpecUrl("/swagger/v1/swagger.json");
            reDoc.DocumentTitle = "MySpot API";
        });
        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        
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