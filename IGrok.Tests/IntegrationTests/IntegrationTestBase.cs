using IGrok.Tests.IntegrationTests;
using System.Net.Http.Headers;

namespace IGrok.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<MyWebApplicationFactory>
{
    protected readonly HttpClient HttpClient;
    protected readonly HttpClient AdminHttpClient;
    protected readonly MyWebApplicationFactory Factory;

    protected IntegrationTestBase(MyWebApplicationFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient();

        AdminHttpClient = factory.CreateClient();
        AdminHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("X-Api-Key", "uTOUCturDEpRoCtorDeNEyApTisHEsONFenTalywcANEOfTSVi");
    }
}
