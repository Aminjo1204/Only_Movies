using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyMoviesProject.Webapi.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace OnlyMoviesProject.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class GenreController : ControllerBase
    {
        private readonly OnlyMoviesContext _db;

        public GenreController(OnlyMoviesContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _db.Genres.ToListAsync();
            if(genres is null || genres.Count == 0)
            {
                return BadRequest("No Genres found");
            }
            var export = genres.Select(a => new
            {
                a.Guid,
                a.Name
            });
            return Ok(export);
        }
    }
}
