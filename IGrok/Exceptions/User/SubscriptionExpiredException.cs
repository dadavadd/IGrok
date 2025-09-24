using System.Net;

namespace IGrok.Exceptions.User;

public class SubscriptionExpiredException : IGrokException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public SubscriptionExpiredException() : base("Subscription has expired") { }
    public SubscriptionExpiredException(DateTime expireTime)
        : base($"Subscription expired on {expireTime:yyyy-MM-dd HH:mm:ss} UTC") { }
}
