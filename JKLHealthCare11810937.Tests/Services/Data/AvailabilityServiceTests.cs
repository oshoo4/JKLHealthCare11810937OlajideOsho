using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Data;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Services.Data
{
    public class AvailabilityServiceTests : IClassFixture<TestStartup>
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityServiceTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _availabilityService = serviceProvider.GetRequiredService<IAvailabilityService>();
        }

        [Theory]
        [InlineData("Tuesday", "15:00", "Wednesday", "16:00", AvailabilityOption.TuesdayAfternoon, true)]
        [InlineData("Wednesday", "09:00", "Thursday", "11:00", AvailabilityOption.WednesdayMorning, true)]
        [InlineData("Monday", "10:00", "Tuesday", "14:00", AvailabilityOption.TuesdayAfternoon, false)]
        [InlineData("Friday", "14:00", "Saturday", "10:00", AvailabilityOption.FridayMorning, false)]
        [InlineData("Thursday", "13:00", "Friday", "17:00", AvailabilityOption.ThursdayAfternoon, true)]
        [InlineData("Saturday", "09:30", "Sunday", "11:30", AvailabilityOption.SaturdayMorning, true)]
        [InlineData("Sunday", "14:00", "Monday", "16:00", AvailabilityOption.SundayAfternoon, true)]
        public void IsCaregiverAvailable_ShouldReturnExpectedResult(
            string startDayOfWeek, string startTime, string endDayOfWeek, string endTime,
            AvailabilityOption availabilityOption, bool expectedResult)
        {
            var caregiver = new Caregiver { Availability = availabilityOption.ToString() };
            string startDate = GetDateForDayOfWeek(startDayOfWeek) + "T" + startTime;
            string endDate = GetDateForDayOfWeek(endDayOfWeek, DateTime.Parse(startDate)) + "T" + endTime;

            bool result = _availabilityService.IsCaregiverAvailable(caregiver, startDate, endDate);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("Monday", "10:00", AvailabilityOption.MondayMorning, true)]
        [InlineData("Monday", "14:00", AvailabilityOption.MondayAfternoon, true)]
        [InlineData("Tuesday", "11:00", AvailabilityOption.TuesdayMorning, true)]
        [InlineData("Tuesday", "16:00", AvailabilityOption.TuesdayAfternoon, true)]
        [InlineData("Monday", "12:30", AvailabilityOption.MondayMorning, false)]
        [InlineData("Saturday", "08:00", AvailabilityOption.SaturdayMorning, false)]
        [InlineData("Sunday", "20:00", AvailabilityOption.SundayAfternoon, false)]
        public void IsCaregiverAvailableForAppointment_ShouldReturnExpectedResult(
            string dayOfWeek, string time, AvailabilityOption availabilityOption, bool expectedResult)
        {
            var caregiver = new Caregiver { Availability = availabilityOption.ToString() };
            string date = GetDateForDayOfWeek(dayOfWeek);

            bool result = _availabilityService.IsCaregiverAvailableForAppointment(caregiver, date, time);

            Assert.Equal(expectedResult, result);
        }

        private string GetDateForDayOfWeek(string dayOfWeek, DateTime? referenceDate = null)
        {
            DayOfWeek day = Enum.Parse<DayOfWeek>(dayOfWeek);
            DateTime today = referenceDate ?? DateTime.Today;

            int daysUntilTargetDay = ((int)day - (int)today.DayOfWeek + 7) % 7;

            if (daysUntilTargetDay == 0)
            {
                daysUntilTargetDay += 7;
            }

            return today.AddDays(daysUntilTargetDay).ToString("yyyy-MM-dd");
        }
    }
}