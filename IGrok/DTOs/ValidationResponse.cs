namespace IGrok.DTOs;

public record ValidationResponse(int UserId,
                                 string Key,
                                 bool IsAccountActive,
                                 DateTime? SubscribeExpireTime);
