using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DBModels
{
    public class Caregiver
    {
        public int CaregiverId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty; 

        public string Qualifications { get; set; } = string.Empty; 

        public string Availability { get; set; } = string.Empty;
    }
}