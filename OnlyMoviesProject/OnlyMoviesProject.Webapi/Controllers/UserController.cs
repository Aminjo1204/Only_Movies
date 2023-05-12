using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using OnlyMoviesProject.Webapi.Infrastructure;
using System.Linq;
using OnlyMoviesProject.Application.Model;
using Microsoft.EntityFrameworkCore;
using OnlyMoviesProject.Application.Dto;
using Microsoft.Extensions.Logging;

namespace OnlyMoviesProject.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]


    public class UserController : ControllerBase
    {
        public record CredentialsDto(string username, string password);

        private readonly OnlyMoviesContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<UserController> _logger;


        public UserController(OnlyMoviesContext db, IConfiguration config, ILogger<UserController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] CredentialsDto credentials)
        {

            var secret = Convert.FromBase64String(_config["Secret"]);
            var lifetime = TimeSpan.FromHours(3);

            var user = _db.Users.FirstOrDefault(u => u.Username == credentials.username);
            if (user is null) { return Unauthorized(); }
            if (!user.CheckPassword(credentials.password)) { return Unauthorized(); }

            var role = user.Role;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
                }),
                Expires = DateTime.UtcNow + lifetime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                user.Username,
                UserGuid = user.Guid,
                IsAdmin = role == Userrole.Admin,
                Token = tokenHandler.WriteToken(token)
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetUserdata()
        {

            var name = HttpContext?.User.Identity?.Name;
            if (name is null) { return Unauthorized(); }


            var user = _db.Users.FirstOrDefault(u => u.Username == name);
            if (user is null) { return Unauthorized(); }
            return Ok(new
            {
                user.Username,
                user.Email,
                user.Role

            });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var user = _db.Users
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.Role
                })
                .ToList();
            if (user is null) { return BadRequest(); }
            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new User(userDto.Username, userDto.Firstname, userDto.Lastname, userDto.Email, userDto.Password, userDto.Role);

                _db.Users.Add(user);
                _db.SaveChanges();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}