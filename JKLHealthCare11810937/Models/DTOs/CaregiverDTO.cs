using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DTOs
{
    public class CaregiverDTO
    {
        public int CaregiverId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty; 

        public string Qualifications { get; set; } = string.Empty; 

        public string Availability { get; set; } = string.Empty;
        
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}