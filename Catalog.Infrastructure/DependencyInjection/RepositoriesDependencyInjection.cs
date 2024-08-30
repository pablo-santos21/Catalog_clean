using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Service;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Identity;
using Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;


namespace Catalog.Infrastructure.DependencyInjection;

public static class RepositoriesDependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryBlogRepository, CategoryBlogRepository>();
        services.AddScoped<ITypeEventRepository, TypeEventRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IApplicationUser, IdentityApplicationUser>();

        return services;
    }
}
