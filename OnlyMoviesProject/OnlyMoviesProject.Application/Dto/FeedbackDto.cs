using System;

namespace OnlyMoviesProject.Application.Dto
{
    public record FeedbackDto(Guid movieGuid, Guid userGuid, string text);
}
