
using GameStore.Api.Data;
using GameStore.Api.Models.Contracts;
using GameStore.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string Racing = "Racing";
    const string Shooter = "Shooter";
    const string GetGame = "GetGame";

    const string ErrorNoGame = "The requested game does not exists";

    private static readonly List<GameContract> games = [
        new (
        1,
        "Polarized user-facing Graphic Interface",
        "RPG",
        19.44M,
        new DateOnly(2016,02,10),
        ""
    ),
    new (
        2,
        "Reactive mission-critical knowledge user",
        "Puzzle",
        11.61M,
        new DateOnly(2021,09,14),
        ""
    ),
    new (
        3,
        "Balanced transitional process improvement",
        Racing,
        10.11M,
        new DateOnly(2016,12,17),
        ""
    ),
    new (
        4,
        "Profit-focused multi-tasking website",
        "Adventure",
        58.95M,
        new DateOnly(2024,11,25),
        ""
    ),
    new (
        5,
        "Optional systemic budgetary management",
        "Sports",
        23.57M,
        new DateOnly(2023,03,30),
        ""
    ),
    new (
        6,
        "Ameliorated object-oriented emulation",
        "RPG",
        60.05M,
        new DateOnly(2020,11,05),
        ""
    ),
    new (
        7,
        "Grass-roots modular projection",
        Shooter,
        35.1M,
        new DateOnly(2023,02,12),
        ""
    ),
    new (
        8,
        "Self-enabling discrete matrices",
        "Adventure",
        82.53M,
        new DateOnly(2020,07,31),
        ""
    ),
    new (
        9,
        "Multi-layered static Graphical User Interface",
        Racing,
        26.66M,
        new DateOnly(2021,09,12),
        ""
    ),
    new (
        10,
        "Monitored 24/7 knowledge base",
        "Sports",
        58.34M,
        new DateOnly(2022,05,14),
        ""
    ),
    new (
        11,
        "Adaptive modular portal",
        "RPG",
        6.16M,
        new DateOnly(2017,05,18),
        ""
    ),
    new (
        12,
        "User-centric neutral knowledge user",
        "Simulation",
        84.74M,
        new DateOnly(2015,06,05),
        ""
    ),
    new (
        13,
        "Implemented solution-oriented extra-net",
        Shooter,
        1.37M,
        new DateOnly(2017,12,12),
        ""
    ),
    new (
        14,
        "Open-source 24/7 initiative",
        Racing,
        41.04M,
        new DateOnly(2016,09,01),
        ""
    ),
    new (
        15,
        "Vision-oriented local monitoring",
        "Sports",
        6.16M,
        new DateOnly(2023,07,02),
        ""
    ),
    new (
        16,
        "Customizable zero-defect benchmark",
        "Simulation",
        79.2M,
        new DateOnly(2020,02,15),
        ""
    ),
    new (
        17,
        "Synergistic hybrid software",
        Racing,
        21.84M,
        new DateOnly(2024,08,31),
        ""
    ),
    new (
        18,
        "Cross-group client-server moderator",
        "RPG",
        57.33M,
        new DateOnly(2020,12,22),
        ""
    ),
    new (
        19,
        "Decentralized needs-based toolset",
        "Puzzle",
        7.44M,
        new DateOnly(2016,04,05),
        ""
    ),
    new (
        20,
        "Synergistic systematic software",
        Racing,
        1.69M,
        new DateOnly(2019,10,14),
        ""
    ),
    new (
        21,
        "Virtual 5th generation array",
        "Puzzle",
        1.65M,
        new DateOnly(2020,02,01),
        ""
    ),
    new (
        22,
        "Optimized optimizing focus group",
        Shooter,
        12.27M,
        new DateOnly(2023,04,28),
        ""
    ),
    new (
        23,
        "Implemented interactive archive",
        "Simulation",
        59.11M,
        new DateOnly(2022,02,05),
        ""
    ),
    new (
        24,
        "Optional incremental website",
        "Adventure",
        61.42M,
        new DateOnly(2022,06,26),
        ""
    ),
    new (
        25,
        "Optimized scalable attitude",
        Shooter,
        71.4M,
        new DateOnly(2023,06,30),
        ""
    )
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder appGroup = app.MapGroup("games").WithParameterValidation();

        // GET /games
        appGroup.MapGet("/", (GameStoreContext dbContext) =>
        {
            return Results.Ok(dbContext.Games.Include(g => g.Genre).ToList());
        });

        // GET /games/{id}
        appGroup.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.FirstOrDefault(g => g.Id == id);

            if (game is null)
            {
                return Results.NotFound(new { Message = ErrorNoGame });
            }
            else
            {
                return Results.Ok(game);
            }
        }).WithName(GetGame);

        // POST /games
        appGroup.MapPost("/", (CreateGameContract newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                Price = newGame.Price,
                ReleasedDate = newGame.ReleasedDate,
                Url = newGame.Url
            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(GetGame, new { id = game.Id }, game);
        });

        // PUT /games/{id}
        appGroup.MapPut("/{id}", (int id, UpdateGameContract updateGame) =>
        {
            int toUpdateIndex = games.FindIndex(g => g.Id == id);
            if (toUpdateIndex != -1)
            {
                GameContract toUpdateGame = new GameContract(games[toUpdateIndex].Id, updateGame.Name, updateGame.Genre, updateGame.Price, updateGame.ReleasedDate, updateGame.Url);
                games[toUpdateIndex] = toUpdateGame;

                return Results.NoContent();
            }
            else
            {
                return Results.BadRequest(new { Message = ErrorNoGame });
            }
        });

        // PUT /games/{id}
        appGroup.MapDelete("/{id}", (int id) =>
        {
            int deleteIndex = games.FindIndex(g => g.Id == id);
            if (deleteIndex != -1)
            {
                games.Remove(games[deleteIndex]);
                return Results.NoContent();
            }
            else
            {
                return Results.NotFound(new { Message = ErrorNoGame });
            }
        });
        return appGroup;
    }

    public static RouteGroupBuilder MapGamesGenreEndpoints(this WebApplication app)
    {
        RouteGroupBuilder appGroup = app.MapGroup("games/genres").WithParameterValidation();

        // GET /games/genres
        appGroup.MapGet("/", (GameStoreContext dbContext) => Results.Ok(dbContext.Genres.ToList()));

        return appGroup;
    }
}
