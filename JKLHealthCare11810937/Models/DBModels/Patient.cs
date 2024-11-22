using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DBModels
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; 

        public string Address { get; set; } = string.Empty;

        [Display(Name = "Medical History")]
        public string MedicalRecords { get; set; } = string.Empty; 
    }
}