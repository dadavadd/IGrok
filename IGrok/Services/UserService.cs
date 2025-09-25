using IGrok.Application;
using IGrok.DTOs;
using IGrok.Exceptions.Token;
using IGrok.Exceptions.User;
using IGrok.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IGrok.Services;

public class UserService(AppDbContext db, ILogger<UserService> logger, IJwtService jwtService) : IUserService
{
    public async Task<List<User>> GetUsersAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        return await db.Users
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<LoginResponse> AuthenticateAsync(string key, string hwid)
    {
        var user = await ValidateLicenseAsync(key, hwid);

        var accessToken = jwtService.GenerateJwtToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, TimeSpan.FromDays(7));
        await db.SaveChangesAsync();

        logger.LogInformation("User {UserKey} authenticated and tokens generated.", key);

        return new(accessToken, refreshToken);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);

        if (principal?.FindFirstValue(ClaimTypes.NameIdentifier) is not { } userIdStr ||
            !int.TryParse(userIdStr, out var userId))
        {
            throw new InvalidRefreshTokenException();
        }

        var user = await db.Users.FindAsync(userId);
        if (user is null || user.RefreshToken != request.RefreshToken)
        {
            logger.LogWarning("Invalid refresh token provided for user ID {UserId}", userId);
            throw new InvalidRefreshTokenException();
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            logger.LogWarning("Expired refresh token provided for user ID {UserId}", userId);
            throw new RefreshTokenExpiredException(user.RefreshTokenExpiryTime!.Value);
        }

        var newAccessToken = jwtService.GenerateJwtToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, TimeSpan.FromDays(7));
        await db.SaveChangesAsync();

        logger.LogInformation("Tokens refreshed for user ID {UserId}", user.Id);
        return new(newAccessToken, newRefreshToken);
    }

    public async Task<ValidationResponse> CreateUserAsync(string key, int? subscriptionDays)
    {
        if (await db.Users.AnyAsync(u => u.Key == key))
        {
            throw new UserAlreadyExistsException(key);
        }

        var newUser = User.Create(key, subscriptionDays);

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        logger.LogInformation("User with key {UserKey} created successfully. ID: {UserId}", newUser.Key, newUser.Id);
        return new(newUser.Id, newUser.Key, newUser.IsActive, newUser.SubscribeExpireTime);
    }

    public async Task<ValidationResponse> UpdateUserHwidAsync(string key, string? newHwid)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Key == key);

        if (user is null)
        {
            throw new UserNotFoundException(key);
        }

        string oldHwid = user.Hwid!;

        user.UpdateHwid(newHwid);
        await db.SaveChangesAsync();

        logger.LogInformation("HWID for user {UserKey} was updated from '{OldHwid}' to '{NewHwid}'", key, oldHwid ?? "NULL", newHwid);

        return new(user.Id, user.Key, user.IsActive, user.SubscribeExpireTime);
    }

    private async Task<User> ValidateLicenseAsync(string key, string hwid)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Key == key);

        if (user is null)
        {
            throw new UserNotFoundException(key);
        }

        if (user.IsSubscriptionExpired())
        {
            logger.LogWarning("Expired subscription attempt for user {UserKey}. Expired on {ExpireTime}", key, user.SubscribeExpireTime);
            throw new SubscriptionExpiredException(user.SubscribeExpireTime!.Value);
        }

        user.ValidateAndBindHwidOnLogin(hwid);

        if (db.Entry(user).State == EntityState.Modified)
        {
            await db.SaveChangesAsync();
        }

        logger.LogInformation("User {UserKey} successfully validated license with HWID {Hwid}", key, hwid);

        return user;
    }

    public async Task SetUserActivationStatusAsync(string key, bool isActive)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Key == key);
        if (user is null)
        {
            throw new UserNotFoundException(key);
        }

        if (isActive)
        {
            user.Activate();
            logger.LogInformation("User {UserKey} has been ACTIVATED.", key);
        }
        else
        {
            user.Deactivate();
            logger.LogInformation("User {UserKey} has been DEACTIVATED.", key);
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(string key)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Key == key);
        if (user is null)
        {
            logger.LogWarning("Attempted to delete non-existent user with key {UserKey}.", key);
            return;
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        logger.LogCritical("User {UserKey} with ID {UserId} has been PERMANENTLY DELETED.", key, user.Id);
    }
}