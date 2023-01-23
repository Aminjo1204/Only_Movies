using AutoMapper;
using OnlyMoviesProject.Application.Model;

namespace OnlyMoviesProject.Application.Dto
{
    public class MappingProfile : Profile  // using AutoMapper;
    {
        public MappingProfile()
        {
            CreateMap<MovieDto, Movie>()           // ArticleDto --> Article
                .ForMember(dest=>dest.Actors, act=>act.Ignore())
                .ForMember(dest=>dest.Genres, act=>act.Ignore());
        }
    }
}
