using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GamesBooking.Domain.Entities;
using GamesBooking.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GamesBooking.IntegrationTests.Endpoints;

public class GameDatabaseTests : IClassFixture<GamesBookingWebApplicationFactory>
{
    private readonly GamesBookingWebApplicationFactory _factory;

    public GameDatabaseTests(GamesBookingWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetGames_ShouldReturnGamesWithSubscribedAndSubstitutePlayers()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/games");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<Game>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games);

        Assert.Equal(2, games.Count);

        var firstGame = games.Single(g => g.Date == new DateOnly(2026, 6, 12));

        Assert.Equal(new DateOnly(2026, 6, 12), firstGame.Date);
        Assert.Equal(new TimeOnly(7, 0), firstGame.Time);
        Assert.Equal(TimeSpan.FromMinutes(90), firstGame.Duration);

        Assert.NotNull(firstGame.SubscribedPlayers);
        Assert.Equal(10, firstGame.SubscribedPlayers.Count);
        Assert.Contains(firstGame.SubscribedPlayers, p => p.Name == "Alex Ovechkin" && p.Role == PlayerRole.Normal);
        Assert.Contains(firstGame.SubscribedPlayers, p => p.Name == "Carey Price" && p.Role == PlayerRole.Goaler);

        Assert.NotNull(firstGame.SubstitutePlayers);
        Assert.Empty(firstGame.SubstitutePlayers);
    }
}
