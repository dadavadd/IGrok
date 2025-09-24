using System.Net;

namespace IGrok.Exceptions.User;

public class UserAlreadyExistsException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

    public UserAlreadyExistsException(string key) : base($"User with key '{key}' already exists") { }
}
