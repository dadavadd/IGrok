using IGrok.Application;
using IGrok.DTOs;
using IGrok.DTOs.Admin;
using IGrok.DTOs.Configs;
using IGrok.Models;
using IGrok.Tests.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IGrok.IntegrationTests.Tests;

public class ConfigTests(MyWebApplicationFactory factory) : IClassFixture<MyWebApplicationFactory>
{
    private const string ApiKey = "uTOUCturDEpRoCtorDeNEyApTisHEsONFenTalywcANEOfTSVi";

    private async Task<string> GetAuthToken(HttpClient client, string userKey, string hwid)
    {
        var loginRequest = new LoginRequest(userKey, hwid);
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return loginResponse!.AccessToken;
    }

    [Fact]
    public async Task GetConfigs_WhenAuthenticated_ReturnsOk()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        var client = factory.CreateClient();
        var userKey = "config-test-user-1";
        var hwid = "hwid-1";

        var adminClient = factory.CreateClient();
        adminClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
        await adminClient.PostAsJsonAsync("/api/v1/admin/users", new CreateUserRequest(userKey, 1));

        var token = await GetAuthToken(client, userKey, hwid);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/configs");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateConfig_WhenAuthenticated_ReturnsCreated()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        var client = factory.CreateClient();
        var userKey = "config-test-user-2";
        var hwid = "hwid-2";

        var adminClient = factory.CreateClient();
        adminClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
        await adminClient.PostAsJsonAsync("/api/v1/admin/users", new CreateUserRequest(userKey, 1));

        var token = await GetAuthToken(client, userKey, hwid);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var newConfig = new CreateConfigRequest("My Test Config", "{\"setting\":\"value\"}");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/configs", newConfig);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdConfig = await response.Content.ReadFromJsonAsync<Config>();
        Assert.NotNull(createdConfig);
        Assert.Equal(newConfig.Name, createdConfig.Name);
    }

    [Fact]
    public async Task GetConfigs_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/configs");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}