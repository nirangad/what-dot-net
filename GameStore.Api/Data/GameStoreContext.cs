using System;
using GameStore.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Battle Royale" },
            new Genre { Id = 2, Name = "Sandbox" },
            new Genre { Id = 3, Name = "MOBA" },
            new Genre { Id = 4, Name = "FPS" },
            new Genre { Id = 5, Name = "Sports" },
            new Genre { Id = 6, Name = "Battle Royale/Party" },
            new Genre { Id = 7, Name = "Social Deduction" },
            new Genre { Id = 8, Name = "Action RPG" },
            new Genre { Id = 9, Name = "MMORPG" },
            new Genre { Id = 10, Name = "FPS/MMORPG" },
            new Genre { Id = 11, Name = "Adventure" },
            new Genre { Id = 12, Name = "PvPvE Shooter" },
            new Genre { Id = 13, Name = "Card Game" },
            new Genre { Id = 14, Name = "Tactical Shooter" },
            new Genre { Id = 15, Name = "Horror/Co-op" }
        );
    }
}
