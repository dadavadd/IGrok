using System.ComponentModel.DataAnnotations;

namespace IGrok.Configurations;

public class AdminSettings
{
    [Required]
	[MinLength(32)]
	public required string ApiKey { get; init; }
}
