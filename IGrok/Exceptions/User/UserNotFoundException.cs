using System.Net;

namespace IGrok.Exceptions.User;

public class UserNotFoundException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public UserNotFoundException() : base("User not found") { }
    public UserNotFoundException(string key) : base($"User with key '{key}' not found") { }
}