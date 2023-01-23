using System;
using System.ComponentModel.DataAnnotations;

namespace OnlyMoviesProject.Application.Dto
{
    public record MovieDto(
            string title, string imdbId, string plot, int? lengthSec, DateTime releaseDate,
            string? rated, string[] genres, string[] actors,
            [Url] string? imageUrl = null);
}
