using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Services.Repository;
using JKLHealthCare11810937.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthCare11810937.Controllers
{
    [Authorize(Roles = "caregiver")]
    public class AppointmentsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IAvailabilityService _availabilityService;
        private readonly IEncryptionService _encryptionService;

        public AppointmentsController(
            IRepository repository,
            IAvailabilityService availabilityService,
            IEncryptionService encryptionService
        )
        {
            _repository = repository;
            _availabilityService = availabilityService;
            _encryptionService = encryptionService;
        }

        public async Task<IActionResult> Index()
        {
            int caregiverId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var appointments = await _repository.GetAppointmentsByCaregiverIdAsync(caregiverId);

            return View(appointments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var allAppointments = await _repository.GetAllAppointmentsAsync();
            if (id == null || allAppointments.Count == 0)
            {
                return NotFound();
            }

            var appointment = await _repository.GetAppointmentByIdAsync((int)id);

            if (appointment == null)
            {
                return NotFound();
            }

            var patient = await _repository.GetPatientByIdAsync(appointment.PatientId);
            if (patient != null)
            {
                string decryptedMedicalRecords = _encryptionService.Decrypt(patient.MedicalRecords);
                ViewBag.MedicalRecords = decryptedMedicalRecords;
            }

            return View(appointment);
        }

        public async Task<IActionResult> Create()
        {
            int caregiverId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var assignedPatients = await _repository.GetPatientsByCaregiverIdAsync(caregiverId);

            ViewData["PatientId"] = new SelectList(assignedPatients, "PatientId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PatientId,Date,Time,Status")]
        Appointment appointment)
        {
            int caregiverId = HttpContext.Session.GetInt32("UserId") ?? 0;

            appointment.CaregiverId = caregiverId;

            var caregiver = await _repository.GetCaregiverByIdAsync(caregiverId);
            var patient = await _repository.GetPatientByIdAsync(appointment.PatientId);

            if (caregiver == null || patient == null)
            {
                return NotFound("Caregiver or patient not found.");
            }

            bool isAssigned = await _repository.CheckPatientAssignedToCaregiver(caregiverId, appointment.PatientId);

            if (!isAssigned)
            {
                ModelState.AddModelError("PatientId", "You are not assigned to this patient.");
            }

            bool isAvailable = _availabilityService.IsCaregiverAvailableForAppointment(caregiver, appointment.Date, appointment.Time);

            if (!isAvailable)
            {
                ModelState.AddModelError("CaregiverId", "You are not available at the specified date and time.");
            }

            bool hasOverlappingAppointment = await _repository.CheckOverlappingAppointments(caregiverId, appointment);

            if (hasOverlappingAppointment)
            {
                ModelState.AddModelError(string.Empty, "You already have an appointment scheduled at this time.");
            }

            if (
                appointment.Caregiver != null
                && appointment.Patient != null
                && !string.IsNullOrEmpty(appointment.Date)
                && !string.IsNullOrEmpty(appointment.Time)
                && !string.IsNullOrEmpty(appointment.Status)
                && isAssigned
                && isAvailable
                && !hasOverlappingAppointment
            )
            {
                var newAppointment = new Appointment
                {
                    CaregiverId = caregiver.CaregiverId,
                    PatientId = patient.PatientId,
                    Caregiver = caregiver,
                    Patient = patient,
                    Date = appointment.Date,
                    Time = appointment.Time,
                    Status = appointment.Status
                };
                await _repository.AddAppointmentAsync(newAppointment);
                return RedirectToAction(nameof(Index));
            }

            var assignedPatients = await _repository.GetPatientsByCaregiverIdAsync(caregiverId);
            ViewData["PatientId"] = new SelectList(assignedPatients, "PatientId", "Name", appointment.PatientId);

            return View(appointment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var allAppointments = await _repository.GetAllAppointmentsAsync();
            if (id == null || allAppointments.Count == 0)
            {
                return NotFound();
            }

            var appointment = await _repository.GetAppointmentByIdAsync((int)id);
            if (appointment == null)
            {
                return NotFound();
            }

            int caregiverId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var assignedPatients = await _repository.GetPatientsByCaregiverIdAsync(caregiverId);

            ViewData["PatientId"] = new SelectList(assignedPatients, "PatientId", "Name", appointment.PatientId);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,CaregiverId,PatientId,Date,Time,Status")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            var existingAppointment = await _repository.GetAppointmentByIdAsync(id);

            if (existingAppointment == null)
            {
                return NotFound();
            }

            int caregiverId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (existingAppointment.CaregiverId != caregiverId)
            {
                return BadRequest("Cannot change the caregiver for this appointment.");
            }

            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.Date = appointment.Date;
            existingAppointment.Time = appointment.Time;
            existingAppointment.Status = appointment.Status;

            var caregiver = await _repository.GetCaregiverByIdAsync(caregiverId);
            var patient = await _repository.GetPatientByIdAsync(existingAppointment.PatientId);

            if (caregiver == null || patient == null)
            {
                return NotFound("Caregiver or patient not found.");
            }

            bool isAssigned = await _repository.CheckPatientAssignedToCaregiver(caregiverId, existingAppointment.PatientId);

            if (!isAssigned)
            {
                ModelState.AddModelError("PatientId", "You are not assigned to this patient.");
            }

            bool isAvailable = _availabilityService.IsCaregiverAvailableForAppointment(caregiver, appointment.Date, appointment.Time);

            if (!isAvailable)
            {
                ModelState.AddModelError("CaregiverId", "You are not available at the specified date and time.");
            }

            bool hasOverlappingAppointment = await _repository.CheckOverlappingAppointments(caregiverId, existingAppointment);

            if (hasOverlappingAppointment)
            {
                ModelState.AddModelError(string.Empty, "You already have an appointment scheduled at this time.");
            }

            if (
                appointment.Caregiver != null
                && appointment.Patient != null
                && !string.IsNullOrEmpty(appointment.Date)
                && !string.IsNullOrEmpty(appointment.Time)
                && !string.IsNullOrEmpty(appointment.Status)
                && isAssigned
                && isAvailable
                && !hasOverlappingAppointment
            )
            {
                try
                {
                    await _repository.UpdateAppointmentAsync(existingAppointment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.AppointmentExists(existingAppointment.AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var assignedPatients = await _repository.GetPatientsByCaregiverIdAsync(caregiverId);
            ViewData["PatientId"] = new SelectList(assignedPatients, "PatientId", "Name", existingAppointment.PatientId);

            return View(existingAppointment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var allAppointments = await _repository.GetAllAppointmentsAsync();
            if (id == null || allAppointments.Count == 0)
            {
                return NotFound();
            }

            var appointment = await _repository.GetAppointmentByIdAsync((int)id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allAppointments = await _repository.GetAllAppointmentsAsync();
            if (allAppointments.Count == 0)
            {
                return Problem("Entity set 'JKLHealthCareContext.Appointments'  is null.");
            }
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                await _repository.DeleteAppointmentAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}