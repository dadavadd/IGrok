using IGrok.Application;
using IGrok.Handlers;
using IGrok.Services;
using Microsoft.EntityFrameworkCore;

namespace IGrok.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IConfigService, ConfigService>();

        return services;
    }
}
