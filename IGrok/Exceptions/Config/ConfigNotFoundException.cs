using System.Net;

namespace IGrok.Exceptions.Config;

public class ConfigNotFoundException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public ConfigNotFoundException() : base("Config not found") { }
    public ConfigNotFoundException(string key) : base($"Config with key '{key}' not found") { }
}
