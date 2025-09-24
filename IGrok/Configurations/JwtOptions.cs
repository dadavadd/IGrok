using System.ComponentModel.DataAnnotations;

namespace IGrok.Configurations;

public class JwtOptions
{
    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    [MinLength(32)]
    public required string SecretKey { get; set; }
}
