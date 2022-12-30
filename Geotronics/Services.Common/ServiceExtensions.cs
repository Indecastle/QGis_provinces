using Geotronics.Services.Geotronics;

namespace Geotronics.Services.Common;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGeotronicsService, GeotronicsService>();

        return services;
    }
}