using System.ComponentModel.DataAnnotations;

namespace JKLHealthCare11810937.Models.DBModels
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public int CaregiverId { get; set; }

        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        public string Date { get; set; } = string.Empty; 

        [Required]
        public string Time { get; set; } = string.Empty; 

        public string Status { get; set; } = string.Empty; 

        public Caregiver Caregiver { get; set; } = new Caregiver();

        public Patient Patient { get; set; } = new Patient();
    }
}