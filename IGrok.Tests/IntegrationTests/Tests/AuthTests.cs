using IGrok.Application;
using IGrok.DTOs;
using IGrok.Models;
using IGrok.Tests.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace IGrok.IntegrationTests.Tests;

public class AuthTests(MyWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Login_WithValidUser_ReturnsOkWithTokens()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var user = User.Create("auth-test-user-key", subscriptionDays: 7);
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new LoginRequest(user.Key, "HWID-ABC-123");

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tokens = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
    }
}