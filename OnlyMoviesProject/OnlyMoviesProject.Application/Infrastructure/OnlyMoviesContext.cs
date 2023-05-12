using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using OnlyMoviesProject.Application.Model;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace OnlyMoviesProject.Webapi.Infrastructure
{
    public class OnlyMoviesContext : DbContext
    {
        public OnlyMoviesContext(DbContextOptions opt) : base(opt) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Genre> Genres => Set<Genre>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();

            // Generic config for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1039);
            var faker = new Faker("de");
            var users = Users.OrderBy(u => u.Email).ToList();

            var genres = new Genre[]
            {
                new Genre(name: "Action"),
                new Genre(name: "Drama"),
                new Genre(name: "Fantasy"),
            };
            Genres.AddRange(genres);
            SaveChanges();

            var images = new string[]
            {
                "https://m.media-amazon.com/images/M/MV5BNjA3NGExZDktNDlhZC00NjYyLTgwNmUtZWUzMDYwMTZjZWUyXkEyXkFqcGdeQXVyMTU1MDM3NDk0._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BYjhiNjBlODctY2ZiOC00YjVlLWFlNzAtNTVhNzM1YjI1NzMxXkEyXkFqcGdeQXVyMjQxNTE1MDA@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BZGQ1ZTNmNzItNGYyMC00MDk2LWJiZDAtZTkwZDFlNWJlYTVjXkEyXkFqcGdeQXVyODUxNDExNTg@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BMzFkZTMzOGUtOGM3NS00YzI2LTllMjgtODk0NDhkNWRiMTMzXkEyXkFqcGdeQXVyNzI1NzMxNzM@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BYjk4ZDAxN2MtYjhlNy00MzJhLWI1MGYtYjY5ZGJlY2YwMzNlXkEyXkFqcGdeQXVyNTc0NjY1ODk@._V1_SX300.jpg",
            };
            var ratings = new string[] { "G", "PG", "PG-13", "R", "NC-17" };
            var movies = new Faker<Movie>("de").CustomInstantiator(f =>
            {
                var releaseDate = new DateTime(1999, 1, 1).AddDays(f.Random.Int(0, 10 * 365));
                var movieActors = new Faker<Actor>("de")
                        .CustomInstantiator(f => new Actor(f.Person.FullName)).Generate(f.Random.Int(1, 3)).ToList();
                var movieGenres = f.Random.ListItems(genres, f.Random.Int(1, 3));
                var movie = new Movie(
                    title: f.Random.Word(),
                    imdbId: f.Random.String2(10, "t0123456789"),
                    plot: f.Lorem.Sentence(f.Random.Int(5, 10)),
                    length: TimeSpan.FromSeconds(f.Random.Int(1 * 3600, 3 * 3600)),
                    releaseDate: releaseDate,
                    rated: f.Random.ListItem(ratings).OrNull(f, 0.5f),
                    imageUrl: f.Random.ListItem(images).OrNull(f, 0.2f),
                    created: releaseDate.AddSeconds(f.Random.Int(7 * 86400, 2 * 365 * 86400)))
                { Guid = f.Random.Guid() };
                movie.Actors.AddRange(movieActors);
                movie.Genres.AddRange(movieGenres);
                return movie;
            })
            .Generate(3)
            .GroupBy(m => m.Title).Select(g => g.First())
            .ToList();
            Movies.AddRange(movies);
            SaveChanges();
            var feedbacks = new Faker<Feedback>("de").CustomInstantiator(f =>
            {
                var movie = f.Random.ListItem(movies);
                return new Feedback(
                   created: movie.Created.AddSeconds(f.Random.Int(86400, 2 * 365 * 86400)),
                   user: f.Random.ListItem(users),
                   movie: movie,
                   text: faker.Random.Words())
                { Guid = f.Random.Guid() };
            })
            .Generate(15)
            .ToList();
            Feedbacks.AddRange(feedbacks);
            SaveChanges();
        }

        /// <summary>
        /// Initialize the database with some values (holidays, ...).
        /// Unlike Seed, this method is also called in production.
        /// </summary>

        private void Initialize()
        {
            // Create a user with the specified attributes
            var user = new User(username: "admin", firstname: "The", lastname: "Admin",
                    email: "admin@spengergasse.at", password: "1111", role: Userrole.Admin);

            var user1 = new User(username: "user", firstname: "The", lastname: "user",
                    email: "user@spengergasse.at", password: "1111", role: Userrole.User);

            // Save the user to the database
            Users.Add(user);
            Users.Add(user1);

            SaveChanges();
        }



        


        public void CreateDatabase(bool isDevelopment)
        {
            if (isDevelopment) { Database.EnsureDeleted(); }
            // EnsureCreated only creates the model if the database does not exist or it has no
            // tables. Returns true if the schema was created.  Returns false if there are
            // existing tables in the database. This avoids initializing multiple times.
            if (Database.EnsureCreated()) { Initialize(); }
            if (isDevelopment) Seed();
        }

    }
}