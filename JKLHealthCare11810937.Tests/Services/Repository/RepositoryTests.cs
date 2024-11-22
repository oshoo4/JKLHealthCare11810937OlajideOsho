using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Repository;
using JKLHealthCare11810937.Services.Security;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JKLHealthCare11810937.Tests.Services.Repository
{
    public class RepositoryTests : IClassFixture<TestStartup>
    {
        private readonly IRepository _repository;
        private readonly IEncryptionService _encryptionService;

        public RepositoryTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _repository = serviceProvider.GetRequiredService<IRepository>();
            _encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();
        }

        [Fact]
        public async Task AddPatientAsync_ShouldAddPatientToDatabase()
        {
            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };

            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            Assert.NotNull(retrievedPatient);
            Assert.Equal("Test Patient", retrievedPatient.Name);
            Assert.Equal("Test Address", retrievedPatient.Address);
            Assert.Equal("Test Medical Records", _encryptionService.Decrypt(retrievedPatient.MedicalRecords));
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnListOfPatients()
        {
            await _repository.AddPatientAsync(
                new Patient
                {
                    Name = "Patient 1",
                    Address = "Address 1",
                    MedicalRecords = _encryptionService.Encrypt("Records 1")
                }
            );
            await _repository.AddPatientAsync(
                new Patient
                {
                    Name = "Patient 2",
                    Address = "Address 2",
                    MedicalRecords = _encryptionService.Encrypt("Records 2")
                }
            );

            var patients = await _repository.GetAllPatientsAsync();

            Assert.NotNull(patients);
            Assert.NotEmpty(patients);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ExistingId_ShouldReturnPatient()
        {
            var patient = new Patient
            {
                Name = "Patient 3",
                Address = "Address 3",
                MedicalRecords = _encryptionService.Encrypt("Records 3")
            };
            await _repository.AddPatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);

            Assert.NotNull(retrievedPatient);
            Assert.Equal(patient.PatientId, retrievedPatient.PatientId);
        }

        [Fact]
        public async Task GetPatientByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var retrievedPatient = await _repository.GetPatientByIdAsync(999);

            Assert.Null(retrievedPatient);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldUpdatePatientInDatabase()
        {
            var patient = new Patient
            {
                Name = "Patient 4",
                Address = "Address 4",
                MedicalRecords = _encryptionService.Encrypt("Records 4")
            };
            await _repository.AddPatientAsync(patient);

            patient.Name = "Updated Patient";
            await _repository.UpdatePatientAsync(patient);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            Assert.Equal("Updated Patient", retrievedPatient?.Name ?? "");
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldDeletePatientFromDatabase()
        {
            var patient = new Patient
            {
                Name = "Patient 5",
                Address = "Address 5",
                MedicalRecords = _encryptionService.Encrypt("Records 5")
            };
            await _repository.AddPatientAsync(patient);

            await _repository.DeletePatientAsync(patient.PatientId);

            var retrievedPatient = await _repository.GetPatientByIdAsync(patient.PatientId);
            Assert.Null(retrievedPatient);
        }

        [Fact]
        public async Task AddCaregiverAsync_ShouldAddCaregiverToDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };

            await _repository.AddCaregiverAsync(caregiver);

            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(caregiver.CaregiverId);
            Assert.NotNull(retrievedCaregiver);
            Assert.Equal("Test Caregiver", retrievedCaregiver.Name);
            Assert.Equal("test.caregiver@email.com", retrievedCaregiver.Contact);
            Assert.Equal("Test Qualifications", retrievedCaregiver.Qualifications);
            Assert.Equal("MondayMorning", retrievedCaregiver.Availability.ToString());
        }

        [Fact]
        public async Task GetAllCaregiversAsync_ShouldReturnListOfCaregivers()
        {
            await _repository.AddCaregiverAsync(new Caregiver
            {
                Name = "Caregiver 1",
                Contact = "contact1@email.com",
                Qualifications = "Qualifications 1",
                Availability = AvailabilityOption.TuesdayAfternoon.ToString()
            });
            await _repository.AddCaregiverAsync(new Caregiver
            {
                Name = "Caregiver 2",
                Contact = "contact2@email.com",
                Qualifications = "Qualifications 2",
                Availability = AvailabilityOption.WednesdayMorning.ToString()
            });


            var caregivers = await _repository.GetAllCaregiversAsync();

            Assert.NotNull(caregivers);
            Assert.NotEmpty(caregivers);
        }

        [Fact]
        public async Task GetCaregiverByIdAsync_ExistingId_ShouldReturnCaregiver()
        {
            var caregiver = new Caregiver
            {
                Name = "Caregiver 3",
                Contact = "contact3@email.com",
                Qualifications = "Qualifications 3",
                Availability = AvailabilityOption.ThursdayAfternoon.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(caregiver.CaregiverId);

            Assert.NotNull(retrievedCaregiver);
            Assert.Equal(caregiver.CaregiverId, retrievedCaregiver.CaregiverId);
        }

        [Fact]
        public async Task GetCaregiverByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(9999);

            Assert.Null(retrievedCaregiver);
        }

        [Fact]
        public async Task UpdateCaregiverAsync_ShouldUpdateCaregiverInDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Caregiver 4",
                Contact = "contact4@email.com",
                Qualifications = "Qualifications 4",
                Availability = AvailabilityOption.FridayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            caregiver.Name = "Updated Caregiver";
            caregiver.Availability = AvailabilityOption.FridayAfternoon.ToString();
            await _repository.UpdateCaregiverAsync(caregiver);

            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(caregiver.CaregiverId);
            Assert.Equal("Updated Caregiver", retrievedCaregiver?.Name ?? "");
            Assert.Equal(AvailabilityOption.FridayAfternoon.ToString(), retrievedCaregiver?.Availability ?? "");
        }

        [Fact]
        public async Task DeleteCaregiverAsync_ShouldDeleteCaregiverFromDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Caregiver 5",
                Contact = "contact5@email.com",
                Qualifications = "Qualifications 5",
                Availability = AvailabilityOption.SaturdayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            await _repository.DeleteCaregiverAsync(caregiver.CaregiverId);

            var retrievedCaregiver = await _repository.GetCaregiverByIdAsync(caregiver.CaregiverId);
            Assert.Null(retrievedCaregiver);
        }

        [Fact]
        public async Task AddAssignmentAsync_ShouldAddAssignmentToDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(7).ToString()
            };

            await _repository.AddAssignmentAsync(assignment);

            var retrievedAssignment = await _repository.GetAssignmentByIdAsync(assignment.AssignmentId);
            Assert.NotNull(retrievedAssignment);
        }

        [Fact]
        public async Task GetAllAssignmentsAsync_ShouldReturnListOfAssignments()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            await _repository.AddAssignmentAsync(new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            });
            await _repository.AddAssignmentAsync(new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.AddDays(8).ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-ddTHH:mm")
            });

            var assignments = await _repository.GetAllAssignmentsAsync();

            Assert.NotNull(assignments);
            Assert.NotEmpty(assignments);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ExistingId_ShouldReturnAssignment()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            };
            await _repository.AddAssignmentAsync(assignment);

            var retrievedAssignment = await _repository.GetAssignmentByIdAsync(assignment.AssignmentId);

            Assert.NotNull(retrievedAssignment);
            Assert.Equal(assignment.AssignmentId, retrievedAssignment.AssignmentId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var retrievedAssignment = await _repository.GetAssignmentByIdAsync(9999);

            Assert.Null(retrievedAssignment);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldUpdateAssignmentInDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            };
            await _repository.AddAssignmentAsync(assignment);

            assignment.EndDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-ddTHH:mm");
            await _repository.UpdateAssignmentAsync(assignment);

            var retrievedAssignment = await _repository.GetAssignmentByIdAsync(assignment.AssignmentId);
            Assert.Equal(assignment.EndDate, retrievedAssignment?.EndDate ?? "");
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ShouldDeleteAssignmentFromDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var assignment = new Assignment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                StartDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm")
            };
            await _repository.AddAssignmentAsync(assignment);

            await _repository.DeleteAssignmentAsync(assignment.AssignmentId);

            var retrievedAssignment = await _repository.GetAssignmentByIdAsync(assignment.AssignmentId);
            Assert.Null(retrievedAssignment);
        }

        [Fact]
        public async Task AddAppointmentAsync_ShouldAddAppointmentToDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var appointment = new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            };

            await _repository.AddAppointmentAsync(appointment);

            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(appointment.AppointmentId);
            Assert.NotNull(retrievedAppointment);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ShouldReturnListOfAppointments()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            await _repository.AddAppointmentAsync(new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            });
            await _repository.AddAppointmentAsync(new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                Time = "14:00",
                Status = "Pending"
            });

            var appointments = await _repository.GetAllAppointmentsAsync();

            Assert.NotNull(appointments);
            Assert.NotEmpty(appointments);
        }

        [Fact]
        public async Task GetAppointmentsByCaregiverIdAsync_ShouldReturnListOfAppointmentsForCaregiver()
        {
            var caregiver1 = new Caregiver
            {
                Name = "Test Caregiver 1",
                Contact = "test.caregiver1@email.com",
                Qualifications = "Test Qualifications 1",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver1);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            await _repository.AddAppointmentAsync(new Appointment
            {
                CaregiverId = caregiver1.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            });

            var appointments = await _repository.GetAppointmentsByCaregiverIdAsync(caregiver1.CaregiverId);

            Assert.NotNull(appointments);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ExistingId_ShouldReturnAppointment()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var appointment = new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            };
            await _repository.AddAppointmentAsync(appointment);

            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(appointment.AppointmentId);

            Assert.NotNull(retrievedAppointment);
            Assert.Equal(appointment.AppointmentId, retrievedAppointment.AppointmentId);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(999);

            Assert.Null(retrievedAppointment);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_ShouldUpdateAppointmentInDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var appointment = new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            };
            await _repository.AddAppointmentAsync(appointment);

            appointment.Status = "Completed";
            await _repository.UpdateAppointmentAsync(appointment);

            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(appointment.AppointmentId);
            Assert.Equal("Completed", retrievedAppointment?.Status ?? "");
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldDeleteAppointmentFromDatabase()
        {
            var caregiver = new Caregiver
            {
                Name = "Test Caregiver",
                Contact = "test.caregiver@email.com",
                Qualifications = "Test Qualifications",
                Availability = AvailabilityOption.MondayMorning.ToString()
            };
            await _repository.AddCaregiverAsync(caregiver);

            var patient = new Patient
            {
                Name = "Test Patient",
                Address = "Test Address",
                MedicalRecords = _encryptionService.Encrypt("Test Medical Records")
            };
            await _repository.AddPatientAsync(patient);

            var appointment = new Appointment
            {
                CaregiverId = caregiver.CaregiverId,
                PatientId = patient.PatientId,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = "10:00",
                Status = "Scheduled"
            };
            await _repository.AddAppointmentAsync(appointment);

            await _repository.DeleteAppointmentAsync(appointment.AppointmentId);

            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(appointment.AppointmentId);
            Assert.Null(retrievedAppointment);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            var user = new User
            {
                Username = "testuser",
                PasswordHash = "hashed_password",
                Role = "caregiver"
            };

            await _repository.AddUserAsync(user);

            var retrievedUser = await _repository.GetUserByUsernameAsync("testuser");
            Assert.NotNull(retrievedUser);
            Assert.Equal("testuser", retrievedUser.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUsername_ShouldReturnUser()
        {
            var user = new User
            {
                Username = "testuser2",
                PasswordHash = "hashed_password2",
                Role = "administrator"
            };
            await _repository.AddUserAsync(user);

            var retrievedUser = await _repository.GetUserByUsernameAsync("testuser2");

            Assert.NotNull(retrievedUser);
            Assert.Equal(user.UserId, retrievedUser.UserId);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var retrievedUser = await _repository.GetUserById(999);

            Assert.Null(retrievedUser);
        }

        [Fact]
        public async Task AdminExists_AdminUserExists_ShouldReturnTrue()
        {
            var user = new User
            {
                Username = "adminuser",
                PasswordHash = "hashed_password",
                Role = "administrator"
            };
            await _repository.AddUserAsync(user);

            bool result = await _repository.AdminExists();

            Assert.True(result);
        }
    }
}