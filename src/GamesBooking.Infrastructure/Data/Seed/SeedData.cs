namespace GamesBooking.Infrastructure.Data.Seed;

public class SeedData
{
    public List<SeedPlayer> Players { get; set; } = [];
    public List<SeedGame> Games { get; set; } = [];
}

public class SeedPlayer
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class SeedGame
{
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<int> SubscribedPlayersId { get; set; } = [];
    public List<int> SubstitutePlayersId { get; set; } = [];
    public List<int> SubscribedGoalersId { get; set; } = [];
    public List<int> SubstituteGoalersId { get; set; } = [];
}
