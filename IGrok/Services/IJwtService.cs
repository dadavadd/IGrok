using IGrok.Application;
using IGrok.DTOs;
using IGrok.Models;
using System.Security.Claims;

namespace IGrok.Services;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}