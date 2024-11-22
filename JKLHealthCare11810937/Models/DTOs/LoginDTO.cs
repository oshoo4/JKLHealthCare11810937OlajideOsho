using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; 
    }
}