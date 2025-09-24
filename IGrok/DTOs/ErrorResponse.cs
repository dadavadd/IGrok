namespace IGrok.DTOs;

public record ErrorResponse(string ErrorCode,
                            string Message,
                            DateTime Timestamp,
                            string? TraceId = null);
