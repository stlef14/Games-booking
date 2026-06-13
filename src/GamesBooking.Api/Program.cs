using GamesBooking.Domain.Repositories;
using GamesBooking.Infrastructure;
using GamesBooking.Infrastructure.Data;
using GamesBooking.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GamesBookingDbContext>();
    await context.Database.EnsureCreatedAsync();

    var seedPath = DatabaseSeeder.ResolveSeedPath(app.Environment.ContentRootPath);
    await DatabaseSeeder.SeedAsync(context, seedPath);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Games Booking API",
    endpoints = new[] { "/api/games" }
}));

app.MapGet("/api/games", async (IGameRepository gameRepository, CancellationToken cancellationToken) =>
{
    var games = await gameRepository.GetAllAsync(cancellationToken);
    return Results.Ok(games);
});

app.Run();

public partial class Program { }
