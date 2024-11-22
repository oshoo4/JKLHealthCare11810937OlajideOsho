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
    public class PatientsControllerTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IEncryptionService _encryptionService;
        private PatientsController systemUnderTest;

        public PatientsControllerTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();
            systemUnderTest = new PatientsController(_repository, _encryptionService);
        }

        [Fact]
        public async Task Index_ReturnsViewWithListOfPatients()
        {
            var patient1 = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            var patient2 = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient1);
            await _repository.AddPatientAsync(patient2);

            var result = await systemUnderTest.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Patient>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithPatientDetails()
        {
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var result = await systemUnderTest.Details(patient.PatientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Patient>(viewResult.ViewData.Model);

            Assert.Equal("Test Medical Records", model.MedicalRecords);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Details(40000);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            var patientDto = new PatientDTO
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };

            var result = await systemUnderTest.Create(patientDto);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            var patientDto = new PatientDTO
            {
                Name = String.Empty,
                Address = "New Address",
                MedicalRecords = _encryptionService.Encrypt("New Records")
            };
            systemUnderTest.ModelState.AddModelError("Name", "The Name field is required.");

            var result = await systemUnderTest.Create(patientDto);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PatientDTO>(viewResult.ViewData.Model);
            Assert.False(systemUnderTest.ModelState.IsValid);

        }

        [Fact]
        public async Task Edit_ValidId_ReturnsViewWithPatient()
        {
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            var result = await systemUnderTest.Edit(retrievedPatient?.PatientId ?? 9999);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Patient>(viewResult.ViewData.Model);

            Assert.Equal("Test Medical Records", model.MedicalRecords);
        }

        [Fact]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var result = await systemUnderTest.Edit(99999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ValidModel_RedirectsToIndex()
        {
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            if (retrievedPatient == null)
            {
                Assert.Fail();
            }
            var result = await systemUnderTest.Edit(retrievedPatient.PatientId, retrievedPatient);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }

        [Fact]
        public async Task Delete_ValidId_ReturnsViewWithPatient()
        {
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            var result = await systemUnderTest.Delete(retrievedPatient?.PatientId ?? 9999);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Patient>(viewResult.ViewData.Model);

            Assert.Equal("Test Medical Records", model.MedicalRecords);
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
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            var result = await systemUnderTest.DeleteConfirmed(retrievedPatient?.PatientId ?? 9999);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }
    }
}