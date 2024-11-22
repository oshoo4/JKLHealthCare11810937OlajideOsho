using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;

namespace JKLHealthCare11810937.Services.Data
{
    public class AvailabilityService : IAvailabilityService
    {
        public bool IsCaregiverAvailableForAppointment(Caregiver caregiver, string date, string time)
        {
            DateTime appointmentDateTime = DateTime.Parse($"{date} {time}");

            if (!Enum.TryParse<AvailabilityOption>(caregiver.Availability, out var availabilityOption))
            {
                return false;
            }

            bool isAvailable = availabilityOption switch
            {
                AvailabilityOption.MondayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Monday && IsMorning(appointmentDateTime),
                AvailabilityOption.MondayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Monday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.TuesdayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Tuesday && IsMorning(appointmentDateTime),
                AvailabilityOption.TuesdayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Tuesday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.WednesdayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Wednesday && IsMorning(appointmentDateTime),
                AvailabilityOption.WednesdayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Wednesday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.ThursdayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Thursday && IsMorning(appointmentDateTime),
                AvailabilityOption.ThursdayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Thursday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.FridayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Friday && IsMorning(appointmentDateTime),
                AvailabilityOption.FridayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Friday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.SaturdayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Saturday && IsMorning(appointmentDateTime),
                AvailabilityOption.SaturdayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Saturday && IsAfternoon(appointmentDateTime),
                AvailabilityOption.SundayMorning => appointmentDateTime.DayOfWeek == DayOfWeek.Sunday && IsMorning(appointmentDateTime),
                AvailabilityOption.SundayAfternoon => appointmentDateTime.DayOfWeek == DayOfWeek.Sunday && IsAfternoon(appointmentDateTime),
                _ => false
            };

            return isAvailable;
        }

        public bool IsCaregiverAvailable(Caregiver caregiver, string startDate, string endDate)
        {
            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate);

            if (!Enum.TryParse<AvailabilityOption>(caregiver.Availability, out var availabilityOption))
            {
                return false;
            }

            bool isAvailable = false;

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                DayOfWeek dayOfWeek = date.DayOfWeek;

                isAvailable = availabilityOption switch
                {
                    AvailabilityOption.MondayMorning => dayOfWeek == DayOfWeek.Monday && IsMorning(date),
                    AvailabilityOption.MondayAfternoon => dayOfWeek == DayOfWeek.Monday && IsAfternoon(date),
                    AvailabilityOption.TuesdayMorning => dayOfWeek == DayOfWeek.Tuesday && IsMorning(date),
                    AvailabilityOption.TuesdayAfternoon => dayOfWeek == DayOfWeek.Tuesday && IsAfternoon(date),
                    AvailabilityOption.WednesdayMorning => dayOfWeek == DayOfWeek.Wednesday && IsMorning(date),
                    AvailabilityOption.WednesdayAfternoon => dayOfWeek == DayOfWeek.Wednesday && IsAfternoon(date),
                    AvailabilityOption.ThursdayMorning => dayOfWeek == DayOfWeek.Thursday && IsMorning(date),
                    AvailabilityOption.ThursdayAfternoon => dayOfWeek == DayOfWeek.Thursday && IsAfternoon(date),
                    AvailabilityOption.FridayMorning => dayOfWeek == DayOfWeek.Friday && IsMorning(date),
                    AvailabilityOption.FridayAfternoon => dayOfWeek == DayOfWeek.Friday && IsAfternoon(date),
                    AvailabilityOption.SaturdayMorning => dayOfWeek == DayOfWeek.Saturday && IsMorning(date),
                    AvailabilityOption.SaturdayAfternoon => dayOfWeek == DayOfWeek.Saturday && IsAfternoon(date),
                    AvailabilityOption.SundayMorning => dayOfWeek == DayOfWeek.Sunday && IsMorning(date),
                    AvailabilityOption.SundayAfternoon => dayOfWeek == DayOfWeek.Sunday && IsAfternoon(date),
                    _ => false
                };

                if (isAvailable)
                {
                    break;
                }
            }

            return isAvailable;
        }

        private bool IsMorning(DateTime date)
        {
            TimeSpan morningStart = new TimeSpan(9, 0, 0);
            TimeSpan morningEnd = new TimeSpan(12, 0, 0);
            return date.TimeOfDay >= morningStart && date.TimeOfDay < morningEnd;
        }

        private bool IsAfternoon(DateTime date)
        {
            TimeSpan afternoonStart = new TimeSpan(13, 0, 0);
            TimeSpan afternoonEnd = new TimeSpan(17, 0, 0);
            return date.TimeOfDay >= afternoonStart && date.TimeOfDay < afternoonEnd;
        }
    }
}