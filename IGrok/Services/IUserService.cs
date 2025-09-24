using IGrok.DTOs;

namespace IGrok.Services;

public interface IUserService
{
    Task<LoginResponse> AuthenticateAsync(string key, string hwid);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ValidationResponse> CreateUserAsync(string key, int? subscriptionDays);
    Task<ValidationResponse> UpdateUserHwidAsync(string key, string? newHwid);
    Task SetUserActivationStatusAsync(string key, bool isActive);
    Task DeleteUserAsync(string key);
}
