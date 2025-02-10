using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("GameStore")
    ?? throw new ArgumentException("Failed to load the connection to database");

builder.Services.AddSqlite<GameStoreContext>(connectionString);
Console.WriteLine($"Connection String: {connectionString}");

var app = builder.Build();

// GET /
app.MapGet("/", () =>
{
    DateTime now = DateTime.Now;
    return Results.Ok($"Welcome to the Game Store 2025 | {now}");
});

// GET /about
app.MapGet("/about", () =>
{
    DateTime now = DateTime.Now;
    return Results.Ok($"Game Store is the go-to place for the latest video games, consoles, accessories, and collectibles, catering to all types of gamers. [{now}]");
});

// GET POST PUT DELETE games
app.MapGamesEndpoints();
// GET POST PUT DELETE genres
app.MapGamesGenreEndpoints();

app.MigrateDb();

await app.RunAsync();
