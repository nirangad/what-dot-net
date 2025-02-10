namespace GameStore.Api.Models.Contracts;

public record class GameContract(int Id, string Name, string Genre, decimal Price, DateOnly? ReleasedDate, string? Url);
