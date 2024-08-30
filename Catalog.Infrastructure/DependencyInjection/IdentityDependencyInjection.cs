using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.DependencyInjection;

public static class IdentityDependencyInjection
{
    public static IServiceCollection AddConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<IdentityApplicationUser, IdentityRole>(options =>
        {
            //configurando identity para user e regras
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>() //configurando o EF como mecanismo para armazenar os dados relacionados ao context
            .AddDefaultTokenProviders(); //adicionando os provedores de tokens padrao para lidar com a autenticação

        return services;
    }
}
