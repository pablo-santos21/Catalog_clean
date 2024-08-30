using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Catalog.Infrastructure.DependencyInjection;

public static class RateLimitingDependencyInjection
{
    public static IServiceCollection AddConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitingConfig = configuration.GetSection("RateLimiting");

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var role = context.User.FindFirst("role")?.Value ?? "Default";
                var config = rateLimitingConfig.GetSection(role);

                return RateLimitPartition.GetTokenBucketLimiter(role, _ =>
                {
                    return new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = config.GetValue<int>("TokenLimit"),
                        TokensPerPeriod = config.GetValue<int>("TokensPerPeriod"),
                        ReplenishmentPeriod = TimeSpan.FromSeconds(config.GetValue<int>("ReplenishmentPeriodInSeconds")),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = config.GetValue<int>("QueueLimit"),
                        AutoReplenishment = true
                    };
                });
            });
        });

        return services;
    }
}
