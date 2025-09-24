using System.ComponentModel.DataAnnotations;

namespace IGrok.DTOs;

public record LoginRequest(
    [Required(ErrorMessage = "Key is required.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Key must be between 8 and 50 characters.")] 
    string Key,

    [Required(ErrorMessage = "HWID is required.")]
    [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "HWID contains invalid characters.")]
    string Hwid
);
