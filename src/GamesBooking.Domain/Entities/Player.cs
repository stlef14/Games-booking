using GamesBooking.Domain.Enums;

namespace GamesBooking.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlayerRole Role { get; set; }
}
