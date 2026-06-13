using System.Text.RegularExpressions;
using GamesBooking.Domain.Entities;
using GamesBooking.Domain.Enums;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GamesBooking.Infrastructure.Data.Seed;

public static class DatabaseSeeder
{
    private static readonly Regex DurationPattern = new(
        @"^(?<value>\d+)\s*(?<unit>minutes?|hours?|hrs?)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string ResolveSeedPath(string contentRootPath)
    {
        var candidates = new[]
        {
            Path.Combine(contentRootPath, "seed.yaml"),
            Path.Combine(AppContext.BaseDirectory, "seed.yaml"),
            Path.GetFullPath(Path.Combine(contentRootPath, "..", "..", "seed.yaml"))
        };

        return candidates.FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException(
                "Could not find seed.yaml.",
                string.Join(", ", candidates));
    }

    public static async Task SeedAsync(
        GamesBookingDbContext context,
        string seedFilePath,
        CancellationToken cancellationToken = default)
    {
        if (context.Players.Any())
        {
            return;
        }

        var yaml = await File.ReadAllTextAsync(seedFilePath, cancellationToken);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        var seedData = deserializer.Deserialize<SeedData>(yaml)
            ?? throw new InvalidOperationException("Seed file is empty or invalid.");

        var playersBySeedId = seedData.Players.ToDictionary(
            seedPlayer => int.Parse(seedPlayer.Id),
            seedPlayer => new Player
            {
                Id = ToPlayerGuid(int.Parse(seedPlayer.Id)),
                Name = seedPlayer.Name,
                Role = Enum.Parse<PlayerRole>(seedPlayer.Role, ignoreCase: true)
            });

        context.Players.AddRange(playersBySeedId.Values);

        foreach (var seedGame in seedData.Games)
        {
            var subscribedIds = seedGame.SubscribedPlayersId.Concat(seedGame.SubscribedGoalersId);
            var substituteIds = seedGame.SubstitutePlayersId.Concat(seedGame.SubstituteGoalersId);

            context.Games.Add(new Game
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.Parse(seedGame.Date),
                Time = TimeOnly.Parse(seedGame.Time),
                Duration = ParseDuration(seedGame.Duration),
                SubscribedPlayers = subscribedIds.Select(id => playersBySeedId[id]).ToList(),
                SubstitutePlayers = substituteIds.Select(id => playersBySeedId[id]).ToList()
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Guid ToPlayerGuid(int seedId) =>
        Guid.Parse($"00000000-0000-4000-8000-{seedId:D12}");

    private static TimeSpan ParseDuration(string duration)
    {
        var match = DurationPattern.Match(duration.Trim());
        if (!match.Success)
        {
            throw new FormatException($"Unsupported duration format: '{duration}'.");
        }

        var value = int.Parse(match.Groups["value"].Value);
        var unit = match.Groups["unit"].Value.ToLowerInvariant();

        return unit.StartsWith("hour") || unit.StartsWith("hr")
            ? TimeSpan.FromHours(value)
            : TimeSpan.FromMinutes(value);
    }
}
