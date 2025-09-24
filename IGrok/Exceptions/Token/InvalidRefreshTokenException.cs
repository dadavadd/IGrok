using System.Net;

namespace IGrok.Exceptions.Token;

public class InvalidRefreshTokenException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

    public InvalidRefreshTokenException()
        : base("Invalid refresh token") { }
}
