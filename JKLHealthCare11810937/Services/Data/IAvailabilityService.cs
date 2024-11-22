using JKLHealthCare11810937.Models.DBModels;

namespace JKLHealthCare11810937.Services.Data
{
    public interface IAvailabilityService
    {
        public bool IsCaregiverAvailable(Caregiver caregiver, string startDate, string endDate);
        public bool IsCaregiverAvailableForAppointment(Caregiver caregiver, string date, string time);
    }
}