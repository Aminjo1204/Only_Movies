using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace OnlyMoviesProject.Application.Model
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
#pragma warning disable CS8618
        protected User() { }
#pragma warning restore CS8618
        public User( string username, string firstname, string lastname, string email, string password, Userrole role)
        {
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Role = role;
            SetPassword(password);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public Userrole Role { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        // Hint for the compiler that we initialize some properties in this method.
        [MemberNotNull(nameof(Salt), nameof(PasswordHash))]
        public void SetPassword(string password)
        {
            Salt = GenerateRandomSalt();
            PasswordHash = CalculateHash(password, Salt);
        }
        public bool CheckPassword(string password) => PasswordHash == CalculateHash(password, Salt);
        /// <summary>
        /// Generates a random number with the given length of bits.
        /// </summary>
        /// <param name="length">Default: 128 bits (16 Bytes)</param>
        /// <returns>A base64 encoded string from the byte array.</returns>
        private string GenerateRandomSalt(int length = 128)
        {
            byte[] salt = new byte[length / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rnd =
                System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Calculates a HMACSHA256 hash value with a given salt.
        /// <returns>Base64 encoded hash.</returns>
        private string CalculateHash(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash =
                new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird als Hexstring zurückgegeben.
            return Convert.ToBase64String(hashedData);
        }
    }
}

