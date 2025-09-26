using System.Net;

namespace IGrok.Exceptions.Config;

public class ConfigAccessDeniedException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public ConfigAccessDeniedException() : base("Access to config denied") { }
    public ConfigAccessDeniedException(string key) : base($"Access to config with key '{key}' denied") { }
}
