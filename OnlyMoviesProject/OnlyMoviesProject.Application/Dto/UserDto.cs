using System;
using System.ComponentModel.DataAnnotations;
using OnlyMoviesProject.Application.Model;

namespace OnlyMoviesProject.Application.Dto
{
    public record UserDto
    (
        int Id,

        Guid Guid,

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Länge des Usernames ist ungültig.")]
        string Username,

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Länge des Firstnames ist ungültig.")]
        string Firstname,

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Länge des Lastnames ist ungültig.")]
        string Lastname,

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Die Länge der Email ist ungültig.")]
        string Email,

        [StringLength(255, MinimumLength = 6, ErrorMessage = "Die Länge des Passworts ist ungültig.")]
        string Password,

        Userrole Role 
    );
}