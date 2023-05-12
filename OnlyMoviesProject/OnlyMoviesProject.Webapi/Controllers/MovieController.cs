using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyMoviesProject.Application.Dto;
using OnlyMoviesProject.Application.Model;
using OnlyMoviesProject.Webapi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlyMoviesProject.Webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class MovieController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OnlyMoviesContext _db;

        public MovieController(OnlyMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("{guid:Guid}")]
        public IActionResult GetMovie(Guid guid)
        {
            var movies = _db.Movies.Where(m => m.Guid == guid).ToList();
            if (!movies.Any()) { return NotFound(); }
            return Ok(GetMovieResult(movies.AsQueryable()).First());
        }

        /// <summary>
        /// Get the last created movies.
        /// GET https://localhost:5001/api/movie/recent
        /// </summary>
        [AllowAnonymous]
        [HttpGet("recent")]
        public IActionResult GetRecentMovies([FromQuery] int? count)
        {
            var itemsCount = Math.Min(10, count ?? 10);
            var moviesResult = GetMovieResult(_db.Movies
                .OrderByDescending(m => m.Created)
                .Take(itemsCount));
            return Ok(moviesResult);
        }

        /// <summary>
        /// Search max. 10 movies with the given title.
        /// GET https://localhost:5001/api/movie/search?title=MyMovie
        /// </summary>
        [AllowAnonymous]
        [HttpGet("search")]
        public IActionResult GetMoviesByTitle([FromQuery] string title)
        {
            title = title.Trim().ToLower();
            if (string.IsNullOrEmpty(title)) { return BadRequest(); }
            if (title.Length < 2) { return BadRequest(); }
            var moviesResult = GetMovieResult(_db.Movies
                .Where(m => m.Title.ToLower().Contains(title))
                .OrderBy(m => m.Title)
                .Take(10));
            return Ok(moviesResult);
        }

        /// <summary>
        /// Adds a movie. If needed the Actors and Genred will be created in the database.
        /// </summary>
        [HttpPost]
        public IActionResult AddMovie(MovieDto movieDto)
        {
            // A movie already exists.
            if (_db.Movies.Any(m => m.ImdbId == movieDto.imdbId)) { return UnprocessableEntity(); }
            // Find out if there are new genres in the submitted movie and add them to the db.
            var existingGenres = _db.Genres.Select(g => g.Name).ToHashSet();
            var newGenres = movieDto.genres.Where(g => !existingGenres.Contains(g)).Select(g => new Genre(name: g)).ToList();
            _db.Genres.AddRange(newGenres);
            // Find out if there are new actors in the submitted movie and add them to the db.
            var existingActors = _db.Actors.Select(a => a.Name).ToHashSet();
            var newActors = movieDto.actors.Where(a => !existingActors.Contains(a)).Select(a => new Actor(name: a)).ToList();
            _db.Actors.AddRange(newActors);
            try { _db.SaveChanges(); }
            catch (DbUpdateException) { return BadRequest("Cannot write new genres or actors to the database."); }

            var movie = _mapper.Map<MovieDto, Movie>(movieDto, opt =>
                opt.AfterMap((dto, entity) =>
                {
                    // Read the db entities of the given actors to get the correct fk.
                    entity.Actors.AddRange(_db.Actors.Where(a => movieDto.actors.Contains(a.Name)));
                    entity.Genres.AddRange(_db.Genres.Where(g => movieDto.genres.Contains(g.Name)));
                    entity.Length = dto.lengthSec.HasValue ? TimeSpan.FromSeconds(dto.lengthSec.Value) : null;
                    entity.Created = DateTime.UtcNow;
                }));
            _db.Movies.Add(movie);
            try { _db.SaveChanges(); }
            catch (DbUpdateException e) { return BadRequest(e.InnerException?.Message ?? e.Message); }
            return CreatedAtAction(nameof(AddMovie), GetMovieResult(new Movie[] { movie }.AsQueryable()));
        }

        [HttpPost("comment")]
        public IActionResult AddMovieFeedback(FeedbackDto feedbackDto)
        {
            var movie = _db.Movies.FirstOrDefault(m => m.Guid == feedbackDto.movieGuid);
            if (movie is null) { return BadRequest(); }
            var user = _db.Users.FirstOrDefault(u => u.Guid == feedbackDto.userGuid);
            if (user is null) { return BadRequest(); }

            var feedback = new Feedback(created: DateTime.UtcNow, user: user, movie: movie, text: feedbackDto.text);
            _db.Feedbacks.Add(feedback);
            try { _db.SaveChanges(); }
            catch (DbUpdateException e) { return BadRequest(e.InnerException?.Message ?? e.Message); }
            return CreatedAtAction(nameof(AddMovieFeedback), new { feedback.Guid });
        }

        //[HttpPost("favourite")]
       
        //public IActionResult AddFavourite(FavouriteDto favouriteDto)
        //{
        //    var movie = _db.Movies.FirstOrDefault(m => m.Guid == favouriteDto.movieGuid);
        //    if (movie is null) { return BadRequest(); }
        //    var user = _db.Users.FirstOrDefault(u => u.Guid == favouriteDto.userGuid);
        //    if (user is null) { return BadRequest(); }

        //    try { _db.SaveChanges(); }
        //    catch (DbUpdateException e) { return BadRequest(e.InnerException?.Message ?? e.Message); }
        //    return CreatedAtAction(nameof(AddMovie), new { favourite.Guid });
        //}

        [Authorize(Roles = "Admin")]
        [HttpDelete("feedback/{guid:Guid}")]
        public IActionResult DeleteFeedback(Guid guid)
        {
            var feedback = _db.Feedbacks.FirstOrDefault(f => f.Guid == guid);
            if (feedback is null) { return NotFound(); }
            _db.Feedbacks.Remove(feedback);
            try { _db.SaveChanges(); }
            catch (DbUpdateException e) { return BadRequest(e.InnerException?.Message ?? e.Message); }
            return NoContent();
        }
        private IEnumerable<object> GetMovieResult(IQueryable<Movie> movies)
        {
            return movies.Select(m => new
            {
                m.Guid,
                m.Title,
                m.ImdbId,
                m.Plot,
                Length = m.Length.HasValue ? m.Length.Value.TotalSeconds : (double?)null,
                ReleaseDate = m.ReleaseDate.ToString("yyyy-MM-dd"),
                m.ReleaseDate.Year,
                m.Rated,
                m.ImageUrl,
                Created = m.Created.ToString("O"),
                Actors = string.Join(", ", m.Actors.OrderBy(a => a.Name).Select(a => a.Name)),
                Genres = string.Join(", ", m.Genres.OrderBy(g => g.Name).Select(g => g.Name)),
                Feedbacks = m.Feedbacks
                    .OrderByDescending(f => f.Created)
                    .Select(f => new
                    {
                        f.Guid,
                        Created = f.Created.ToString("O"),
                        f.User.Username,
                        f.Text
                    })
            })
            .ToList();
        }
    }
}