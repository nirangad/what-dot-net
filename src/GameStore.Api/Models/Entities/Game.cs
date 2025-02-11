using System;

namespace GameStore.Api.Models.Entities;

public class Game
{
    public int Id { get; set;}
    public required string Name { get; set;}
    public int GenreId{ get; set;}
    public Genre? Genre{ get; set;}
    public required decimal Price{ get; set;}
    public DateOnly? ReleasedDate { get; set;}
    public string? Url { get; set;}

}
