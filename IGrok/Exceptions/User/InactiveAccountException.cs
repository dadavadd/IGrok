using System.Net;

namespace IGrok.Exceptions.User;

public class InactiveAccountException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public InactiveAccountException() : base("The user account is inactive") { }
    public InactiveAccountException(string key) : base($"The user account with key '{key}' is inactive") { }
}
