using JKLHealthCare11810937.Controllers;
using JKLHealthCare11810937.Hubs;
using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Services.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Controllers
{
    public class AssignmentsControllerTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IAvailabilityService _availabilityService;
        private readonly AssignmentsController systemUnderTest;

        public AssignmentsControllerTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _availabilityService = serviceProvider.GetRequiredService<IAvailabilityService>();

            var hub = new AssignmentHub();
            var hubContext = (IHubContext<AssignmentHub>)hub.Context;

            systemUnderTest = new AssignmentsController(
                _repository,
                hubContext,
                _availabilityService
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithListOfAssignments()
        {
            var assignment1 = new Assignment
            {
                Caregiver = new Caregiver { Name = "Caregiver 1" },
                Patient = new Patient { Name = "Patient 1" },
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(7).ToString()
            };
            var assignment2 = new Assignment
            {
                Caregiver = new Caregiver { Name = "Caregiver 1" },
                Patient = new Patient { Name = "Patient 2" },
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(7).AddHours(1).ToString()
            };
            await _repository.AddAssignmentAsync(assignment1);
            await _repository.AddAssignmentAsync(assignment2);

            var result = await systemUnderTest.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Assignment>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithAssignmentDetails()
        {
            var assignment = new Assignment
            {
                Caregiver = new Caregiver { Name = "Caregiver 1" },
                Patient = new Patient { Name = "Patient 1" },
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(7).ToString()
            };
            await _repository.AddAssignmentAsync(assignment);

            var result = await systemUnderTest.Details(assignment.AssignmentId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Assignment>(viewResult.ViewData.Model);
            Assert.Equal(assignment, model);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Get_ReturnsViewWithSelectLists()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1" };
            var patient = new Patient { Name = "Patient 1" };
            await _repository.AddCaregiverAsync(caregiver);
            await _repository.AddPatientAsync(patient);

            var result = await systemUnderTest.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<SelectList>(viewResult.ViewData["CaregiverId"]);
            Assert.IsType<SelectList>(viewResult.ViewData["PatientId"]);
        }

        [Fact]
        public async Task Edit_Get_ValidId_ReturnsViewWithAssignment()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            var patient = new Patient { Name = "Patient 1" };
            await _repository.AddCaregiverAsync(caregiver);
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            };
            await _repository.AddAssignmentAsync(assignment);

            var result = await systemUnderTest.Edit(assignment.AssignmentId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Assignment>(viewResult.ViewData.Model);
            Assert.Equal(assignment, model);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsViewWithAssignment()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            var patient = new Patient { Name = "Patient 1" };
            await _repository.AddCaregiverAsync(caregiver);
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            };
            await _repository.AddAssignmentAsync(assignment);

            var result = await systemUnderTest.Delete(assignment.AssignmentId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Assignment>(viewResult.ViewData.Model);
            Assert.Equal(assignment, model);
        }
    }
}