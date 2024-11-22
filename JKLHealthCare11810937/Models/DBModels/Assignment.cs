using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DBModels
{
    public class Assignment
    {
        public int AssignmentId { get; set; }

        [Required]
        public int CaregiverId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; } = string.Empty;

        [Display(Name = "End Date")]
        public string EndDate { get; set; } = string.Empty;

        public Caregiver Caregiver { get; set; } = new Caregiver(); 

        public Patient Patient { get; set; } = new Patient(); 
    }
}