using JKLHealthCare11810937.Models.DTOs;
using JKLHealthCare11810937.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using JKLHealthCare11810937.Controllers;
using JKLHealthCare11810937.Services.Security;
using JKLHealthCare11810937.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;

namespace JKLHealthCare11810937.Tests.Controllers
{
    public class AccountControllerTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IValidationService _validationService;
        private readonly AccountController systemUnderTest;

        public AccountControllerTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _userAuthenticationService = serviceProvider.GetRequiredService<IUserAuthenticationService>();
            _validationService = serviceProvider.GetRequiredService<IValidationService>();

            systemUnderTest = new AccountController(
                _repository,
                _userAuthenticationService,
                _validationService
            );
        }

        [Fact]
        public async Task Register_ValidModel_RedirectsToIndex()
        {
            var registerDto = new RegisterDTO
            {
                Username = "testuser",
                PasswordHash = "Test!1234",
                Role = "caregiver"
            };

            var result = await systemUnderTest.Register(registerDto);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithModel()
        {
            var registerDto = new RegisterDTO
            {
                Username = "testuser",
                PasswordHash = "Test!1234",
                Role = "caregiver"
            };
            systemUnderTest.ModelState.AddModelError("Name", "The Name field is required.");

            var result = await systemUnderTest.Register(registerDto);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<RegisterDTO>(viewResult.ViewData.Model);
            Assert.False(systemUnderTest.ModelState.IsValid);
        }

        [Fact]
        public async Task Register_InvalidUsernameLength_AddsModelError()
        {
            var registerDto = new RegisterDTO
            {
                Username = "short",
                PasswordHash = "Test!1234",
                Role = "caregiver"
            };

            var result = await systemUnderTest.Register(registerDto);

            Assert.False(systemUnderTest.ModelState.IsValid);
            Assert.True(systemUnderTest.ModelState.ContainsKey("Username"));
        }

        [Fact]
        public async Task Register_InvalidPasswordComplexity_AddsModelError()
        {
            var registerDto = new RegisterDTO
            {
                Username = "testuser",
                PasswordHash = "weakpass",
                Role = "caregiver"
            };

            await systemUnderTest.Register(registerDto);

            Assert.False(systemUnderTest.ModelState.IsValid);
            Assert.True(systemUnderTest.ModelState.ContainsKey("PasswordHash"));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithModelError()
        {
            var loginDto = new LoginDTO { Username = "testuser", PasswordHash = "wrongpassword" };

            var result = await systemUnderTest.Login(loginDto);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LoginDTO>(viewResult.ViewData.Model);
            Assert.False(systemUnderTest.ModelState.IsValid);
        }
    }
}