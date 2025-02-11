using System;
using GameStore.Api.Models.Contracts;
using GameStore.Api.Models.Entities;
using Microsoft.AspNetCore.StaticAssets;

namespace GameStore.Api.Mappings;

public static class GameMappings
{
    public static Game toGame(this CreateGameContract contract) => new Game
    {
        Name = contract.Name,
        GenreId = contract.GenreId,
        Price = contract.Price,
        ReleasedDate = contract.ReleasedDate,
        Url = contract.Url
    };

    public static GameContract toGameContract(this Game game)
    {
        return new GameContract(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            game.ReleasedDate,
            game.Url ?? string.Empty
        );
    }

    public static Game assignToGame(this UpdateGameContract contract, Game game)
    {
        game.Name = contract.Name;
        game.Price = contract.Price;
        game.ReleasedDate = contract.ReleasedDate;
        game.Url = contract.Url;

        return game;
    }
}
