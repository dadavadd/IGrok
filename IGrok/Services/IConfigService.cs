using IGrok.DTOs.Shared;
using IGrok.Models;

namespace IGrok.Services;

public interface IConfigService
{
    Task<PaginatedResponse<Config>> GetConfigsByUserIdAsync(int userId, int pageNumber, int pageSize);
    Task<Config> GetConfigByIdAsync(int id, int userId);
    Task<Config> CreateConfigAsync(int userId, string name, string jsonContent);
    Task UpdateConfigAsync(int id, int userId, string name, string jsonContent);
    Task DeleteConfigAsync(int id, int userId);
}
