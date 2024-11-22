using JKLHealthCare11810937.Controllers;
using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Services.Repository;
using JKLHealthCare11810937.Services.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Controllers
{
    public class AppointmentsControllerTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IAvailabilityService _availabilityService;
        private readonly IEncryptionService _encryptionService;
        private readonly AppointmentsController systemUnderTest;

        public AppointmentsControllerTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _availabilityService = serviceProvider.GetRequiredService<IAvailabilityService>();
            _encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();

            systemUnderTest = new AppointmentsController(
                _repository,
                _availabilityService,
                _encryptionService
            );
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithAppointmentDetails()
        {
            var appointment = new Appointment
            {
                CaregiverId = 1,
                PatientId = 1,
                Caregiver = new Caregiver
                {
                    Name = "Caregiver 1",
                    Contact = "caregiver1@email.com",
                    Qualifications = "Registered Nurse",
                    Availability = AvailabilityOption.FridayAfternoon.ToString()
                },
                Patient = new Patient
                {
                    PatientId = 1,
                    Name = "Patient 1",
                    MedicalRecords = _encryptionService.Encrypt("Records")
                }
            };
            await _repository.AddAppointmentAsync(appointment);

            var result = await systemUnderTest.Details(appointment.AppointmentId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Appointment>(viewResult.ViewData.Model);
            Assert.Equal(appointment, model);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsViewWithAppointment()
        {
            var appointment = new Appointment
            {
                CaregiverId = 1,
                PatientId = 1,
                Caregiver = new Caregiver
                {
                    Name = "Caregiver 1",
                    Contact = "caregiver1@email.com",
                    Qualifications = "Registered Nurse",
                    Availability = AvailabilityOption.FridayAfternoon.ToString()
                },
                Patient = new Patient
                {
                    PatientId = 1,
                    Name = "Patient 1",
                    MedicalRecords = _encryptionService.Encrypt("Records")
                }
            };
            await _repository.AddAppointmentAsync(appointment);

            var result = await systemUnderTest.Delete(appointment.AppointmentId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Appointment>(viewResult.ViewData.Model);
            Assert.Equal(appointment, model);
        }
    }
}