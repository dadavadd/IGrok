using IGrok.Configurations;
using IGrok.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace IGrok.Services;

public class JwtService(IOptions<JwtOptions> options) : IJwtService
{
    private readonly JwtOptions _jwtOptions = options.Value;
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(options.Value.SecretKey));
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public string GenerateJwtToken(User user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("userKey", user.Key)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}