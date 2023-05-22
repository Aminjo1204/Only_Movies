using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyMoviesProject.Application.Model
{
    public class Config
    {
        public int Id { get; set; }
        public string? MailerAccountname { get; set; }

        [StringLength(4096)]
        public string? MailerRefreshToken { get; set; }
    }
}