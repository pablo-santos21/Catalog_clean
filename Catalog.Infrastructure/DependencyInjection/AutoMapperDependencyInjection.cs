using Catalog.Infrastructure.Mappings;
using Microsoft.Extensions.DependencyInjection;


namespace Catalog.Infrastructure.DependencyInjection;

public static class AutoMapperDependencyInjection
{
    public static IServiceCollection AddConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ViewsMappingProfile));
        return services;
    }
}
