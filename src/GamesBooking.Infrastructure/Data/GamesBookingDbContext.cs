using GamesBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamesBooking.Infrastructure.Data;

public class GamesBookingDbContext : DbContext
{
    public GamesBookingDbContext(DbContextOptions<GamesBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Role).HasConversion<string>();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.HasMany(g => g.SubscribedPlayers)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("GameSubscribedPlayers"));

            entity.HasMany(g => g.SubstitutePlayers)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("GameSubstitutePlayers"));
        });
    }
}
