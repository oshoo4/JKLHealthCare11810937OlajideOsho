using JKLHealthCare11810937.Controllers;
using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Models.DTOs;
using JKLHealthCare11810937.Services.Repository;
using JKLHealthCare11810937.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Controllers
{
    public class CaregiversControllerTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IValidationService _validationService;
        private readonly CaregiversController systemUnderTest;

        public CaregiversControllerTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _userAuthenticationService = serviceProvider.GetRequiredService<IUserAuthenticationService>();
            _validationService = serviceProvider.GetRequiredService<IValidationService>();

            systemUnderTest = new CaregiversController(
                _repository,
                _userAuthenticationService,
                _validationService
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithListOfCaregivers()
        {
            var caregiver1 = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            var caregiver2 = new Caregiver { Name = "Caregiver 2", Availability = AvailabilityOption.TuesdayAfternoon.ToString() };
            await _repository.AddCaregiverAsync(caregiver1);
            await _repository.AddCaregiverAsync(caregiver2);

            var result = await systemUnderTest.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Caregiver>>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithCaregiverDetails()
        {
            var caregiver1 = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            await _repository.AddCaregiverAsync(caregiver1);

            var result = await systemUnderTest.Details(caregiver1.CaregiverId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Caregiver>(viewResult.ViewData.Model);

            Assert.Equal(caregiver1, model);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            var caregiverDto = new CaregiverDTO { /* ... invalid data ... */ };
            systemUnderTest.ModelState.AddModelError("Name", "The Name field is required.");

            var result = await systemUnderTest.Create(caregiverDto);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CaregiverDTO>(viewResult.ViewData.Model);
            Assert.False(systemUnderTest.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_InvalidUsernameLength_AddsModelError()
        {
            var caregiverDto = new CaregiverDTO { Username = "short", PasswordHash = "Test!1234", Name = "Test Caregiver", Availability = AvailabilityOption.MondayMorning.ToString() };

            var result = await systemUnderTest.Create(caregiverDto);

            Assert.False(systemUnderTest.ModelState.IsValid);
            Assert.True(systemUnderTest.ModelState.ContainsKey("Username"));
        }

        [Fact]
        public async Task Create_InvalidPasswordComplexity_AddsModelError()
        {
            var caregiverDto = new CaregiverDTO { Username = "testuser", PasswordHash = "weakpass", Name = "Test Caregiver", Availability = AvailabilityOption.MondayMorning.ToString() };

            var result = await systemUnderTest.Create(caregiverDto);

            Assert.False(systemUnderTest.ModelState.IsValid);
            Assert.True(systemUnderTest.ModelState.ContainsKey("PasswordHash"));
        }

        [Fact]
        public async Task Edit_ValidId_ReturnsViewWithCaregiver()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            await _repository.AddCaregiverAsync(caregiver);

            var result = await systemUnderTest.Edit(caregiver.CaregiverId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Caregiver>(viewResult.ViewData.Model);

            Assert.Equal(caregiver.CaregiverId, model.CaregiverId);
        }

        [Fact]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ValidModel_RedirectsToIndex()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            await _repository.AddCaregiverAsync(caregiver);

            var result = await systemUnderTest.Edit(caregiver.CaregiverId, caregiver);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Delete(9999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            var caregiver = new Caregiver { Name = "Caregiver 1", Availability = AvailabilityOption.MondayMorning.ToString() };
            await _repository.AddCaregiverAsync(caregiver);

            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(caregiver.CaregiverId);
            var result = await systemUnderTest.DeleteConfirmed(retrievedCaregiver?.CaregiverId ?? 9999);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }
    }
}