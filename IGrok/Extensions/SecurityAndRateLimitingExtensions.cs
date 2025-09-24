using System.Threading.RateLimiting;

namespace IGrok.Extensions;

public static class SecurityAndRateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("sliding-by-ip", httpContext =>
            {
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: remoteIpAddress,
                    factory: _ => new()
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }
}
