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
    public class Feedback
    {
#pragma warning disable CS8618
        protected Feedback() { }
#pragma warning restore CS8618
        public Feedback(DateTime created, User user, Movie movie, string text)
        {
            Created = created;
            User = user;
            Movie = movie;
            Text = text;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        public Movie Movie { get; set; }
        public string Text { get; set; }
    }
}
