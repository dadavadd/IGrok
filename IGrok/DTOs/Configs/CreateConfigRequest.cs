namespace IGrok.DTOs.Configs;

public record CreateConfigRequest(int UserId, string Name, string JsonContent);
