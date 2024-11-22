using JKLHealthCare11810937.Models.DBModels;

namespace JKLHealthCare11810937.Services.Repository
{
    public interface IRepository
    {
        Task<List<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task AddPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(int id);
        Task<List<Patient>> GetPatientsByCaregiverIdAsync(int caregiverId);
        bool PatientExists(Patient patient);
        Task DeletePatientWithAssignmentsAndAppointmentsAsync(int id, Patient patient);

        Task<List<Caregiver>> GetAllCaregiversAsync();
        Task<Caregiver?> GetCaregiverByIdAsync(int id);
        Task AddCaregiverAsync(Caregiver caregiver);
        Task UpdateCaregiverAsync(Caregiver caregiver);
        Task DeleteCaregiverAsync(int id);
        Task DeleteCaregiverAssignmentsAndAppointmentsAsync(Assignment assignment);
        Task DeleteCaregiverWithAssignmentsAndAppointmentsAsync(int id, Caregiver caregiver, User caregiverUser);
        public bool CaregiverExists(int caregiverId);

        Task<List<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task AddAssignmentAsync(Assignment assignment);
        Task UpdateAssignmentAsync(Assignment assignment);
        Task DeleteAssignmentAsync(int id);
        Task<List<Assignment>> GetAllAssignmentsFormattedAsync();
        bool AssignmentExists(int assignmentId);

        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByCaregiverIdAsync(int caregiverId);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int id);
        Task<bool> CheckPatientAssignedToCaregiver(int caregiverId, int patientId);
        Task<bool> CheckOverlappingAppointments(int caregiverId, Appointment appointment);
        bool AppointmentExists(int appointmentId);
        Task<bool> CheckPatientAlreadyAssigned(Assignment assignment, DateTime assignmentStartDate, DateTime assignmentEndDate);

        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserById(int id);
        Task AddUserAsync(User user);
        Task<bool> AdminExists();
    }
}