using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace JKLHealthCare11810937.Models.DBModels
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty; 

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}