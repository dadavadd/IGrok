using IGrok.Models;

namespace IGrok.Tests.UnitTests;

public class UserTests
{
    [Fact]
    public void Create_UserWithSubscription_ShouldSetCorrectExpiryDateAndBeActive()
    {
        // Arrange
        var key = "test-key-123";
        var subscriptionDays = 30;

        // Act
        var user = User.Create(key, subscriptionDays);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(key, user.Key);
        Assert.True(user.IsActive);
        Assert.NotNull(user.SubscribeExpireTime);

        var expectedExpiry = DateTime.UtcNow.AddDays(subscriptionDays);
        Assert.True(user.SubscribeExpireTime.Value > expectedExpiry.AddMinutes(-1));
        Assert.True(user.SubscribeExpireTime.Value < expectedExpiry.AddMinutes(1));
    }

    [Fact]
    public void IsSubscriptionExpired_WhenDateIsInThePast_ShouldReturnTrue()
    {
        // Arrange
        var user = User.Create("expired-user", -10);

        // Act
        var isExpired = user.IsSubscriptionExpired();

        // Assert
        Assert.True(isExpired);
    }

    [Fact]
    public void IsSubscriptionExpired_WhenDateIsInTheFuture_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("active-user", 10);

        // Act
        var isExpired = user.IsSubscriptionExpired();

        // Assert
        Assert.False(isExpired);
    }
}
