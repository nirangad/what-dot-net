using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Models.Contracts;

public record class CreateGameContract(
    [Required][StringLength(100)] string Name,
    int GenreId,
    [Range(1, 500)] decimal Price,
    DateOnly ReleasedDate,
    string Url
);
