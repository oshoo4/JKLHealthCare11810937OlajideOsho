using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JKLHealthCare11810937.Tests.Mocks
{
    public class MockRepository : IRepository
    {
        private readonly List<Patient> _patients = new List<Patient>();
        private readonly List<Caregiver> _caregivers = new List<Caregiver>();
        private readonly List<Assignment> _assignments = new List<Assignment>();
        private readonly List<Appointment> _appointments = new List<Appointment>();
        private readonly List<User> _users = new List<User>();

        private int _patientIdCounter = 1;
        private int _caregiverIdCounter = 1;
        private int _assignmentIdCounter = 1;
        private int _appointmentIdCounter = 1;
        private int _userIdCounter = 1;

        public Task<List<Patient>> GetAllPatientsAsync()
        {
            return Task.FromResult(_patients);
        }

        public Task<Patient?> GetPatientByIdAsync(int id)
        {
            return Task.FromResult(_patients.FirstOrDefault(p => p.PatientId == id));
        }

        public Task AddPatientAsync(Patient patient)
        {
            patient.PatientId = _patientIdCounter++;
            _patients.Add(patient);
            return Task.CompletedTask;
        }

        public Task UpdatePatientAsync(Patient patient)
        {
            var existingPatient = _patients.FirstOrDefault(p => p.PatientId == patient.PatientId);
            if (existingPatient != null)
            {
                existingPatient.Name = patient.Name;
                existingPatient.Address = patient.Address;
                existingPatient.MedicalRecords = patient.MedicalRecords;
            }
            return Task.CompletedTask;
        }

        public Task DeletePatientAsync(int id)
        {
            _patients.RemoveAll(p => p.PatientId == id);
            return Task.CompletedTask;
        }

        public Task<List<Patient>> GetPatientsByCaregiverIdAsync(int caregiverId)
        {
            var patients = _assignments
                .Where(a => a.CaregiverId == caregiverId)
                .Select(a => _patients.FirstOrDefault(p => p.PatientId == a.PatientId))
                .Where(p => p != null)
                .Cast<Patient>()
                .ToList();
            return Task.FromResult(patients);
        }

        public bool PatientExists(Patient patient)
        {
            return _patients.Any(p => p.PatientId == patient.PatientId);
        }

        public Task DeletePatientWithAssignmentsAndAppointmentsAsync(int id, Patient patient)
        {
            _appointments.RemoveAll(a => a.PatientId == id);
            _assignments.RemoveAll(a => a.PatientId == id);
            _patients.Remove(patient);
            return Task.CompletedTask;
        }

        public Task<List<Caregiver>> GetAllCaregiversAsync()
        {
            return Task.FromResult(_caregivers);
        }

        public Task<Caregiver?> GetCaregiverByIdAsync(int id)
        {
            return Task.FromResult(_caregivers.FirstOrDefault(c => c.CaregiverId == id));
        }

        public Task AddCaregiverAsync(Caregiver caregiver)
        {
            caregiver.CaregiverId = _caregiverIdCounter++;
            _caregivers.Add(caregiver);
            return Task.CompletedTask;
        }

        public Task UpdateCaregiverAsync(Caregiver caregiver)
        {
            var existingCaregiver = _caregivers.FirstOrDefault(c => c.CaregiverId == caregiver.CaregiverId);
            if (existingCaregiver != null)
            {
                existingCaregiver.Name = caregiver.Name;
                existingCaregiver.Contact = caregiver.Contact;
                existingCaregiver.Qualifications = caregiver.Qualifications;
                existingCaregiver.Availability = caregiver.Availability;
            }
            return Task.CompletedTask;
        }

        public Task DeleteCaregiverAsync(int id)
        {
            _caregivers.RemoveAll(c => c.CaregiverId == id);
            return Task.CompletedTask;
        }

        public Task DeleteCaregiverAssignmentsAndAppointmentsAsync(Assignment assignment)
        {
            _appointments.RemoveAll(a => a.CaregiverId == assignment.CaregiverId);
            _assignments.Remove(assignment);
            return Task.CompletedTask;
        }

        public Task DeleteCaregiverWithAssignmentsAndAppointmentsAsync(int id, Caregiver caregiver, User caregiverUser)
        {
            _appointments.RemoveAll(a => a.CaregiverId == id);
            _assignments.RemoveAll(a => a.CaregiverId == id);
            _caregivers.Remove(caregiver);
            _users.Remove(caregiverUser);
            return Task.CompletedTask;
        }

        public bool CaregiverExists(int caregiverId)
        {
            return _caregivers.Any(c => c.CaregiverId == caregiverId);
        }

        public Task<List<Assignment>> GetAllAssignmentsAsync()
        {
            return Task.FromResult(_assignments);
        }

        public Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return Task.FromResult(_assignments.FirstOrDefault(a => a.AssignmentId == id));
        }

        public Task AddAssignmentAsync(Assignment assignment)
        {
            assignment.AssignmentId = _assignmentIdCounter++;
            _assignments.Add(assignment);
            return Task.CompletedTask;
        }

        public Task UpdateAssignmentAsync(Assignment assignment)
        {
            var existingAssignment = _assignments.FirstOrDefault(a => a.AssignmentId == assignment.AssignmentId);
            if (existingAssignment != null)
            {
                existingAssignment.CaregiverId = assignment.CaregiverId;
                existingAssignment.PatientId = assignment.PatientId;
                existingAssignment.StartDate = assignment.StartDate;
                existingAssignment.EndDate = assignment.EndDate;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAssignmentAsync(int id)
        {
            _assignments.RemoveAll(a => a.AssignmentId == id);
            return Task.CompletedTask;
        }

        public Task<List<Assignment>> GetAllAssignmentsFormattedAsync()
        {
            return Task.FromResult(_assignments);
        }

        public bool AssignmentExists(int assignmentId)
        {
            return _assignments.Any(a => a.AssignmentId == assignmentId);
        }

        public Task<bool> CheckPatientAlreadyAssigned(Assignment assignment, DateTime assignmentStartDate, DateTime assignmentEndDate)
        {
            return Task.FromResult(true);
        }

        public Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return Task.FromResult(_appointments);
        }

        public Task<List<Appointment>> GetAppointmentsByCaregiverIdAsync(int caregiverId)
        {
            var appointments = _appointments.Where(a => a.CaregiverId == caregiverId).ToList();
            return Task.FromResult(appointments);
        }

        public Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return Task.FromResult(_appointments.FirstOrDefault(a => a.AppointmentId == id));
        }

        public Task AddAppointmentAsync(Appointment appointment)
        {
            appointment.AppointmentId = _appointmentIdCounter++;
            _appointments.Add(appointment);
            return Task.CompletedTask;
        }

        public Task UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = _appointments.FirstOrDefault(a => a.AppointmentId == appointment.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment.CaregiverId = appointment.CaregiverId;
                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.Date = appointment.Date;
                existingAppointment.Time = appointment.Time;
                existingAppointment.Status = appointment.Status;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAppointmentAsync(int id)
        {
            _appointments.RemoveAll(a => a.AppointmentId == id);
            return Task.CompletedTask;
        }

        public Task<bool> CheckPatientAssignedToCaregiver(int caregiverId, int patientId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CheckOverlappingAppointments(int caregiverId, Appointment appointment)
        {
            bool hasOverlappingAppointment = _appointments.Any(a =>
                a.CaregiverId == caregiverId &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time);

            return Task.FromResult(hasOverlappingAppointment);
        }

        public bool AppointmentExists(int appointmentId)
        {
            return _appointments.Any(a => a.AppointmentId == appointmentId);
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }

        public Task<User?> GetUserById(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.UserId == id));
        }

        public Task AddUserAsync(User user)
        {
            user.UserId = _userIdCounter++;
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> AdminExists()
        {
            return Task.FromResult(_users.Any(u => u.Role == "administrator"));
        }
    }
}