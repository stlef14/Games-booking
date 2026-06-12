using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GamesBooking.IntegrationTests;

public class PlaceholderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PlaceholderIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RootEndpoint_ShouldReturnSuccess()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("API Scaffolding Active", content);
    }
}
