using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Models.Contracts;

public record class UpdateGameContract(
    [Required][StringLength(100)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 500)] decimal Price,
    DateOnly ReleasedDate,
    string Url
);
