using System;

namespace OnlyMoviesProject.Application.Dto
{
    public record FavouriteDto(Guid movieGuid, Guid userGuid);
}
