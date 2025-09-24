using System.Net;

namespace IGrok.Exceptions.User;

public class HwidMismatchException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public HwidMismatchException() : base("Hardware ID mismatch") { }
    public HwidMismatchException(string providedHwid)
        : base($"Hardware ID '{providedHwid}' does not match registered device") { }
}
