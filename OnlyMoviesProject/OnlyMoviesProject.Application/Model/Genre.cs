using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace OnlyMoviesProject.Application.Model
{
    [Index(nameof(Name), IsUnique = true)]
    public class Genre
    {
        public Genre(string name)
        {
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; } = new();
    }
}
