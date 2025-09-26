using IGrok.Application;
using IGrok.DTOs.Shared;
using IGrok.Exceptions.Config;
using IGrok.Models;
using Microsoft.EntityFrameworkCore;

namespace IGrok.Services;

public class ConfigService(AppDbContext context, ILogger<ConfigService> logger) : IConfigService
{
    public async Task<PaginatedResponse<Config>> GetConfigsByUserIdAsync(int userId, int pageNumber, int pageSize) 
    {
        logger.LogInformation("Fetching configs for user ID {UserId}, page {PageNumber}, page size {PageSize}", userId, pageNumber, pageSize);

        var query = context.Configs.Where(c => c.UserId == userId);
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Config> GetConfigByIdAsync(int id, int userId) 
    {
        logger.LogInformation("Fetching config with ID {ConfigId} for user ID {UserId}", id, userId);
        var config = await context.Configs.FindAsync(id);
        if (config == null)
        {
            throw new ConfigNotFoundException(id.ToString());
        }

        if (config.UserId != userId)
        {
            throw new ConfigAccessDeniedException(id.ToString());
        }

        return config;
    }

    public async Task<Config> CreateConfigAsync(int userId, string name, string jsonContent)
    {
        var config = Config.Create(userId, name, jsonContent);

        context.Configs.Add(config);
        await context.SaveChangesAsync();

        logger.LogInformation("Created new config with ID {ConfigId} for user ID {UserId}", config.Id, userId);

        return config;
    }

    public async Task UpdateConfigAsync(int id, int userId, string name, string jsonContent)
    {
        var config = await context.Configs.FindAsync(id);
        if (config == null)
        {
            throw new ConfigNotFoundException(id.ToString());
        }

        if (config.UserId != userId)
        {
            throw new ConfigAccessDeniedException(id.ToString());
        }

        config.Update(name, jsonContent);
        await context.SaveChangesAsync();
        logger.LogInformation("Updated config with ID {ConfigId}", id);
    }

    public async Task DeleteConfigAsync(int id, int userId)
    {
        var config = await context.Configs.FindAsync(id);
        if (config == null)
        {
            throw new ConfigNotFoundException(id.ToString());
        }

        if (config.UserId != userId)
        {
            throw new ConfigAccessDeniedException(id.ToString());
        }

        context.Configs.Remove(config);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted config with ID {ConfigId}", id);
    }
}