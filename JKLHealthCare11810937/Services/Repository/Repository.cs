using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;

namespace JKLHealthCare11810937.Services.Repository
{
    public class Repository : IRepository
    {
        private readonly JKLHealthCareContext _context;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public Repository(JKLHealthCareContext context)
        {
            _context = context;

            _circuitBreakerPolicy = Policy
                .Handle<DbUpdateException>()
                .Or<DbUpdateConcurrencyException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Patients.ToListAsync()
            );
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Patients.FindAsync(id)
            );
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            });
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Entry(patient).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeletePatientAsync(int id)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient != null)
                {
                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task<List<Patient>> GetPatientsByCaregiverIdAsync(int caregiverId)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Assignments
                    .Where(a => a.CaregiverId == caregiverId)
                    .Select(a => a.Patient)
                    .ToListAsync()
            );
        }

        public bool PatientExists(Patient patient)
        {
            return _context.Patients.Any(e => e.PatientId == patient.PatientId);
        }

        public async Task DeletePatientWithAssignmentsAndAppointmentsAsync(int id, Patient patient)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var appointmentsToDelete = await _context.Appointments
                    .Where(a => a.PatientId == id)
                    .ToListAsync();
                _context.Appointments.RemoveRange(appointmentsToDelete);

                var assignmentsToDelete = await _context.Assignments
                    .Where(a => a.PatientId == id)
                    .ToListAsync();
                _context.Assignments.RemoveRange(assignmentsToDelete);

                _context.Patients.Remove(patient);

                await _context.SaveChangesAsync();
            });
        }

        public async Task<List<Caregiver>> GetAllCaregiversAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Caregivers.ToListAsync()
            );
        }

        public async Task<Caregiver?> GetCaregiverByIdAsync(int id)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Caregivers.FindAsync(id)
            );
        }

        public async Task AddCaregiverAsync(Caregiver caregiver)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                await _context.Caregivers.AddAsync(caregiver);
                await _context.SaveChangesAsync();
            });
        }

        public async Task UpdateCaregiverAsync(Caregiver caregiver)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Entry(caregiver).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteCaregiverAsync(int id)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var caregiver = await _context.Caregivers.FindAsync(id);
                if (caregiver != null)
                {
                    _context.Caregivers.Remove(caregiver);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task DeleteCaregiverAssignmentsAndAppointmentsAsync(Assignment assignment)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var appointmentsToDelete = await _context.Appointments
                    .Where(a => a.CaregiverId == assignment.CaregiverId)
                    .ToListAsync();
                _context.Appointments.RemoveRange(appointmentsToDelete);

                _context.Assignments.Remove(assignment);

                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteCaregiverWithAssignmentsAndAppointmentsAsync(int id, Caregiver caregiver, User caregiverUser)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var appointmentsToDelete = await _context.Appointments
                    .Where(a => a.CaregiverId == id)
                    .ToListAsync();
                _context.Appointments.RemoveRange(appointmentsToDelete);

                var assignmentsToDelete = await _context.Assignments
                    .Where(a => a.CaregiverId == id)
                    .ToListAsync();
                _context.Assignments.RemoveRange(assignmentsToDelete);
                _context.Caregivers.Remove(caregiver);
                _context.Users.Remove(caregiverUser);

                await _context.SaveChangesAsync();
            });
        }

        public bool CaregiverExists(int caregiverId)
        {
            return _context.Caregivers.Any(e => e.CaregiverId == caregiverId);
        }

        public async Task<List<Assignment>> GetAllAssignmentsAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Assignments
                    .Include(a => a.Caregiver)
                    .Include(a => a.Patient)
                    .ToListAsync()
            );
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Assignments
                    .Include(a => a.Caregiver)
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(m => m.AssignmentId == id)
            );
        }


        public async Task AddAssignmentAsync(Assignment assignment)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();
            });
        }

        public async Task UpdateAssignmentAsync(Assignment assignment)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Entry(assignment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment != null)
                {
                    _context.Assignments.Remove(assignment);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task<List<Assignment>> GetAllAssignmentsFormattedAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Assignments
                .Include(a => a.Caregiver)
                .Include(a => a.Patient)
                .Select(a => new Assignment
                {
                    AssignmentId = a.AssignmentId,
                    CaregiverId = a.CaregiverId,
                    PatientId = a.PatientId,
                    Caregiver = a.Caregiver,
                    Patient = a.Patient,
                    StartDate = DateTime.Parse(a.StartDate).ToString("dddd, dd MMMM yyyy HH:mm"),
                    EndDate = DateTime.Parse(a.EndDate).ToString("dddd, dd MMMM yyyy HH:mm")
                }).ToListAsync()
            );
        }

        public async Task<bool> CheckPatientAlreadyAssigned(Assignment assignment, DateTime assignmentStartDate, DateTime assignmentEndDate)
        {
            return (await _context.Assignments.ToListAsync())
                        .Any(a => a.PatientId == assignment.PatientId &&
                                DateTime.Parse(a.StartDate) <= assignmentEndDate &&
                                DateTime.Parse(a.EndDate) >= assignmentStartDate);
        }

        public bool AssignmentExists(int appointmentId)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == appointmentId)).GetValueOrDefault();
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Appointments
                    .Include(a => a.Caregiver)
                    .Include(a => a.Patient)
                    .ToListAsync()
            );
        }

        public async Task<List<Appointment>> GetAppointmentsByCaregiverIdAsync(int caregiverId)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Appointments
                    .Include(a => a.Caregiver)
                    .Include(a => a.Patient)
                    .Where(a => a.CaregiverId == caregiverId)
                    .ToListAsync()
            );
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Appointments
                    .Include(a => a.Caregiver)
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(m => m.AppointmentId == id)
            );
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
            });
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Entry(appointment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment != null)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task<bool> CheckPatientAssignedToCaregiver(int caregiverId, int patientId)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Assignments
                .AnyAsync(a => a.CaregiverId == caregiverId && a.PatientId == patientId)
            );
        }

        public async Task<bool> CheckOverlappingAppointments(int caregiverId, Appointment appointment)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Appointments
                    .AnyAsync(a => a.CaregiverId == caregiverId &&
                            a.Date == appointment.Date &&
                            a.Time == appointment.Time)
            );
        }

        public bool AppointmentExists(int appointmentId)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == appointmentId)).GetValueOrDefault();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Users.FirstOrDefaultAsync(u => u.Username == username)
            );
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Users.FindAsync(id)
            );
        }

        public async Task AddUserAsync(User user)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            });
        }

        public async Task<bool> AdminExists()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _context.Users.AnyAsync(u => u.Role == "administrator")
            );
        }
    }
}