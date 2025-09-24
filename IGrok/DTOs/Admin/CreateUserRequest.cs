using System.ComponentModel.DataAnnotations;

namespace IGrok.DTOs.Admin;

public record CreateUserRequest(
    [Required]
    [MinLength(8)]
    string Key,
    int? SubscriptionDays = null
);
