using System.ComponentModel.DataAnnotations;

namespace IGrok.DTOs;

public record RefreshTokenRequest(
    [Required] string AccessToken,
    [Required] string RefreshToken
);