using System.Net;

namespace IGrok.Exceptions;

public abstract class IGrokException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }
    public virtual string ErrorCode => GetType().Name;

    protected IGrokException(string message) : base(message) { }
    protected IGrokException(string message, Exception innerException) : base(message, innerException) { }
}
