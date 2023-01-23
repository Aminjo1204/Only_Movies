using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace OnlyMoviesProject.Application.Model
{

    [Index(nameof(Title), IsUnique = true)]
    public class Movie
    {
        public Movie(
            string title, string imdbId, string plot,
            DateTime releaseDate, DateTime created, TimeSpan? length = null, string? rated = null, string? imageUrl = null)
        {
            Title = title;
            ImdbId = imdbId;
            Plot = plot;
            Length = length;
            ReleaseDate = releaseDate;
            Rated = rated;
            ImageUrl = imageUrl;
            Created = created;
        }
#pragma warning disable CS8618
        protected Movie() { }
#pragma warning restore CS8618
        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        [MaxLength(65535)]
        public string Plot { get; set; }
        public TimeSpan? Length { get; set; }
        public DateTime ReleaseDate { get;  set; }
        public string? Rated { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Created { get; set; }
        public List<Actor> Actors { get; } = new();
        public List<Genre> Genres { get; } = new();
        public List<Feedback> Feedbacks { get; } = new();
    }
}
