using System.Collections.Generic;

namespace GamesBooking.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public TimeSpan Duration { get; set; }
    public ICollection<Player> SubscribedPlayers { get; set; } = new List<Player>();
    public ICollection<Player> SubstitutePlayers { get; set; } = new List<Player>();
}
