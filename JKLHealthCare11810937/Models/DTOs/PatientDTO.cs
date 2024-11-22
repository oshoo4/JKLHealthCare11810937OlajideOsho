using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DTOs
{
    public class PatientDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string MedicalRecords { get; set; } = string.Empty;
    }
}