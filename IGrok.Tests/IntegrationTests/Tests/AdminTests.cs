using IGrok.Application;
using IGrok.DTOs;
using IGrok.Tests.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;

namespace IGrok.IntegrationTests.Tests;

public class AdminTests(MyWebApplicationFactory factory) : IClassFixture<MyWebApplicationFactory>
{
    private const string ApiKey = "uTOUCturDEpRoCtorDeNEyApTisHEsONFenTalywcANEOfTSVi";

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

        var response = await client.PostAsJsonAsync("/api/v1/admin/users", new { Key = "user1231231231", SubscriptionDays = 7 }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<ValidationResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(created);
        Assert.Equal("user1231231231", created.Key);
    }
}
