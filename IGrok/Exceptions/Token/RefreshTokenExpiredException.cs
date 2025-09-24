using System.Net;

namespace IGrok.Exceptions.Token;

public class RefreshTokenExpiredException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

    public RefreshTokenExpiredException(DateTime expireTime)
        : base($"Refresh token expired on {expireTime:yyyy-MM-dd HH:mm:ss} UTC") { }
}
