
using GameStore.Api.Data;
using GameStore.Api.Mappings;
using GameStore.Api.Models.Contracts;
using GameStore.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGame = "GetGame";

    const string ErrorNoGame = "The requested game does not exists";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder appGroup = app.MapGroup("games").WithParameterValidation();

        // GET /games
        appGroup.MapGet("/", async (GameStoreContext dbContext) =>
        {
            var gameContracts = await dbContext.Games
            .Include(g => g.Genre)
            .Select(game => game.toGameContract())
            .AsNoTracking()
            .ToListAsync();

            return Results.Ok(gameContracts);
        });

        // GET /games/{id}
        appGroup.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
            {
                return Results.NotFound(new { Message = ErrorNoGame });
            }
            else
            {
                return Results.Ok(game.toGameContract());
            }
        }).WithName(GetGame);

        // POST /games
        appGroup.MapPost("/", async (CreateGameContract newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.toGame();
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameContract gameContract = game.toGameContract();

            return Results.CreatedAtRoute(GetGame, new { id = game.Id }, gameContract);
        });

        // PUT /games/{id}
        appGroup.MapPut("/{id}", async (int id, UpdateGameContract updateGame, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);
            if (game is null)
            {
                return Results.NotFound(new { Message = ErrorNoGame });
            }
            else
            {
                updateGame.assignToGame(game);
                dbContext.SaveChanges();
                return Results.Ok(game.toGameContract());
            }
        });

        // PUT /games/{id}
        appGroup.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            /* 
                Direct Approach
                Trying LINQ directly: dbContext.Games.Where(g => g.Id == id).ExecuteDelete();
            */
            Game? game = await dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game is null)
            {
                return Results.NotFound(new { Message = ErrorNoGame });
            }
            else
            {
                dbContext.Games.Remove(game);
                dbContext.SaveChanges();
                return Results.NoContent();
            }
        });
        return appGroup;
    }

    public static RouteGroupBuilder MapGamesGenreEndpoints(this WebApplication app)
    {
        RouteGroupBuilder appGroup = app.MapGroup("games/genres").WithParameterValidation();

        // GET /games/genres
        appGroup.MapGet("/", async (GameStoreContext dbContext) => Results.Ok(await dbContext.Genres.AsNoTracking().ToListAsync()));

        return appGroup;
    }
}
