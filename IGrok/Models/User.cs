using IGrok.Exceptions.User;

namespace IGrok.Models;

public class User
{
    private User(string key) => Key = key;

    public int Id { get; private set; }
    public string Key { get; private set; }
    public string? Hwid { get; private set; }
    public DateTime? SubscribeExpireTime { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public ICollection<Config> Configs { get; set; } = new List<Config>();

    public static User Create(string key, int? subscriptionDays)
    {
        var user = new User(key)
        {
            SubscribeExpireTime = subscriptionDays.HasValue
                ? DateTime.UtcNow.AddDays(subscriptionDays.Value)
                : null,
            IsActive = true
        };

        return user;
    }

    public void UpdateHwid(string? newHwid)
    {
        if (!IsActive)
        {
            throw new InactiveAccountException(Key);
        }

        Hwid = newHwid;
    }

    public void ValidateAndBindHwidOnLogin(string providedHwid)
    {
        if (!IsActive)
        {
            throw new InactiveAccountException(Key);
        }

        if (string.IsNullOrEmpty(Hwid))
        {
            Hwid = providedHwid;
            return;
        }

        if (Hwid != providedHwid)
        {
            throw new HwidMismatchException(providedHwid);
        }
    }

    public bool IsSubscriptionExpired()
    {
        if (!SubscribeExpireTime.HasValue)
        {
            return false;
        }

        return SubscribeExpireTime.Value < DateTime.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, TimeSpan validity)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = DateTime.UtcNow.Add(validity);
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
    }
}