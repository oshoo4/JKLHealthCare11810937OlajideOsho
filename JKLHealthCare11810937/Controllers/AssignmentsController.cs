using JKLHealthCare11810937.Hubs;
using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Services.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthCare11810937.Controllers
{
    [Authorize(Roles = "administrator")]
    public class AssignmentsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IHubContext<AssignmentHub> _hubContext;
        private readonly IAvailabilityService _availabilityService;

        public AssignmentsController(
            IRepository repository,
            IHubContext<AssignmentHub> hubContext,
            IAvailabilityService availabilityService
        )
        {
            _repository = repository;
            _hubContext = hubContext;
            _availabilityService = availabilityService;
        }

        public async Task<IActionResult> Index()
        {
            var allAssignmentsFormatted = await _repository.GetAllAssignmentsFormattedAsync();
            return View(allAssignmentsFormatted);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var allAssignments = await _repository.GetAllAssignmentsAsync();
            if (id == null || allAssignments.Count == 0)
            {
                return NotFound();
            }

            var assignment = await _repository.GetAssignmentByIdAsync((int)id);
            if (assignment == null)
            {
                return NotFound();
            }

            DateTime startDateTime = DateTime.Parse(assignment.StartDate);
            DateTime endDateTime = DateTime.Parse(assignment.EndDate);

            assignment.StartDate = startDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            assignment.EndDate = endDateTime.ToString("dddd, dd MMMM yyyy HH:mm");

            return View(assignment);
        }

        public async Task<IActionResult> Create()
        {
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            var allPatients = await _repository.GetAllPatientsAsync();
            ViewData["CaregiverId"] = new SelectList(allCaregivers, "CaregiverId", "Name");
            ViewData["PatientId"] = new SelectList(allPatients, "PatientId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignmentId,CaregiverId,PatientId,StartDate,EndDate")] Assignment assignment)
        {
            var caregiver = await _repository.GetCaregiverByIdAsync(assignment.CaregiverId);
            var patient = await _repository.GetPatientByIdAsync(assignment.PatientId);

            if (caregiver == null || patient == null)
            {
                return NotFound("Caregiver or patient not found.");
            }

            assignment.Caregiver = caregiver;
            assignment.Patient = patient;

            if (
                assignment.Caregiver != null &&
                assignment.Patient != null &&
                !string.IsNullOrEmpty(assignment.StartDate) &&
                !string.IsNullOrEmpty(assignment.EndDate)
            )
            {
                bool isCaregiverAvailable = _availabilityService.IsCaregiverAvailable(caregiver, assignment.StartDate, assignment.EndDate);

                if (!isCaregiverAvailable)
                {
                    string caregiverName = assignment.Caregiver.Name;
                    AvailabilityOption availability = Enum.Parse<AvailabilityOption>(assignment.Caregiver.Availability);
                    string availabilityValue = availability.GetDisplayName();
                    ModelState.AddModelError("CaregiverId", $"{caregiverName} is only available {availabilityValue}.");
                }

                DateTime assignmentStartDate = DateTime.Parse(assignment.StartDate);
                DateTime assignmentEndDate = DateTime.Parse(assignment.EndDate);
                bool isPatientAssigned = await _repository.CheckPatientAlreadyAssigned(assignment, assignmentStartDate, assignmentEndDate);

                if (isPatientAssigned)
                {
                    ModelState.AddModelError("PatientId", "Patient is already assigned to another caregiver during this time.");
                }

                if (!isPatientAssigned && isCaregiverAvailable)
                {
                    var patientName = assignment.Patient.Name;
                    var caregiverId = assignment.CaregiverId;

                    await _repository.AddAssignmentAsync(assignment);

                    string message = $"{{ \"caregiverId\": \"{caregiverId}\", \"message\": \"You have been assigned to {patientName}.\" }}";
                    await _hubContext.Clients.All
                        .SendAsync("ReceiveAssignmentNotification", message);

                    return RedirectToAction(nameof(Index));
                }
            }

            var allCaregivers = await _repository.GetAllCaregiversAsync();
            var allPatients = await _repository.GetAllPatientsAsync();
            ViewData["CaregiverId"] = new SelectList(allCaregivers, "CaregiverId", "Name", assignment.CaregiverId);
            ViewData["PatientId"] = new SelectList(allPatients, "PatientId", "Name", assignment.PatientId);
            return View(assignment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var allAssignments = await _repository.GetAllAssignmentsAsync();
            if (id == null || allAssignments.Count == 0)
            {
                return NotFound();
            }

            var assignment = await _repository.GetAssignmentByIdAsync((int)id);
            if (assignment == null)
            {
                return NotFound();
            }
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            var allPatients = await _repository.GetAllPatientsAsync();
            ViewData["CaregiverId"] = new SelectList(allCaregivers, "CaregiverId", "Name", assignment.CaregiverId);
            ViewData["PatientId"] = new SelectList(allPatients, "PatientId", "Name", assignment.PatientId);
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentId,CaregiverId,PatientId,StartDate,EndDate")] Assignment assignment)
        {
            if (id != assignment.AssignmentId)
            {
                return NotFound();
            }

            var caregiver = await _repository.GetCaregiverByIdAsync(assignment.CaregiverId);
            var patient = await _repository.GetPatientByIdAsync(assignment.PatientId);

            if (caregiver == null || patient == null)
            {
                return NotFound("Caregiver or patient not found.");
            }

            if (
                caregiver != null &&
                patient != null &&
                !string.IsNullOrEmpty(assignment.StartDate) &&
                !string.IsNullOrEmpty(assignment.EndDate)
            )
            {
                bool isCaregiverAvailable = _availabilityService.IsCaregiverAvailable(caregiver, assignment.StartDate, assignment.EndDate);

                if (!isCaregiverAvailable)
                {
                    string caregiverName = caregiver.Name;
                    AvailabilityOption availability = Enum.Parse<AvailabilityOption>(caregiver.Availability);
                    string availabilityValue = availability.GetDisplayName();
                    ModelState.AddModelError("CaregiverId", $"{caregiverName} is only available {availabilityValue}.");
                }

                DateTime assignmentStartDate = DateTime.Parse(assignment.StartDate);
                DateTime assignmentEndDate = DateTime.Parse(assignment.EndDate);
                bool isPatientAssigned = await _repository.CheckPatientAlreadyAssigned(assignment, assignmentStartDate, assignmentEndDate);

                if (isPatientAssigned && isCaregiverAvailable)
                {
                    var assignedToSameCaregiver = assignment.PatientId == patient.PatientId && assignment.CaregiverId == caregiver.CaregiverId;
                    if (assignedToSameCaregiver)
                    {
                        if (!isCaregiverAvailable)
                        {
                            string caregiverName = caregiver.Name;
                            AvailabilityOption availability = Enum.Parse<AvailabilityOption>(caregiver.Availability);
                            string availabilityValue = availability.GetDisplayName();
                            ModelState.AddModelError("CaregiverId", $"{caregiverName} is only available {availabilityValue}.");
                        }
                        try
                        {
                            var existingAssignment = await _repository.GetAssignmentByIdAsync(id);
                            if (existingAssignment == null)
                            {
                                return NotFound();
                            }
                            UpdateAssignmentAndSendNotification(
                                existingAssignment,
                                assignment,
                                caregiver,
                                patient
                            );
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!_repository.AssignmentExists(assignment.AssignmentId))
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
                    else
                    {
                        ModelState.AddModelError("PatientId", "Patient is already assigned to another caregiver during this time.");
                    }
                }

                if (!isPatientAssigned && isCaregiverAvailable)
                {
                    try
                    {
                        var existingAssignment = await _repository.GetAssignmentByIdAsync(id);
                        if (existingAssignment == null)
                        {
                            return NotFound();
                        }
                        UpdateAssignmentAndSendNotification(
                            existingAssignment,
                            assignment,
                            caregiver,
                            patient
                        );
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_repository.AssignmentExists(assignment.AssignmentId))
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
            }

            var allCaregivers = await _repository.GetAllCaregiversAsync();
            var allPatients = await _repository.GetAllPatientsAsync();
            ViewData["CaregiverId"] = new SelectList(allCaregivers, "CaregiverId", "Name", assignment.CaregiverId);
            ViewData["PatientId"] = new SelectList(allPatients, "PatientId", "Name", assignment.PatientId);
            return View(assignment);
        }

        private async void UpdateAssignmentAndSendNotification(
            Assignment existingAssignment,
            Assignment newAssignment,
            Caregiver caregiver,
            Patient patient
        )
        {
            var patientName = existingAssignment.Patient.Name;
            var caregiverId = existingAssignment.CaregiverId;

            existingAssignment.Caregiver = caregiver;
            existingAssignment.Patient = patient;
            existingAssignment.StartDate = newAssignment.StartDate;
            existingAssignment.EndDate = newAssignment.EndDate;
            existingAssignment.PatientId = newAssignment.PatientId;
            existingAssignment.CaregiverId = newAssignment.CaregiverId;

            await _repository.UpdateAssignmentAsync(existingAssignment);

            string message = $"{{ \"caregiverId\": \"{caregiverId}\", \"message\": \"Your assignment with {patientName} has been updated.\" }}";
            await _hubContext.Clients.All
                .SendAsync("ReceiveAssignmentNotification", message);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var allAssignments = await _repository.GetAllAssignmentsAsync();
            if (id == null || allAssignments.Count == 0)
            {
                return NotFound();
            }

            var assignment = await _repository.GetAssignmentByIdAsync((int)id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allAssignments = await _repository.GetAllAssignmentsAsync();
            if (allAssignments.Count == 0)
            {
                return Problem("Entity set 'JKLHealthCareContext.Assignments'  is null.");
            }
            var assignment = await _repository.GetAssignmentByIdAsync(id);

            if (assignment != null)
            {
                var patient = await _repository.GetPatientByIdAsync(assignment.PatientId);
                var patientName = patient?.Name ?? assignment.PatientId.ToString();
                var caregiverId = assignment.CaregiverId;

                await _repository.DeleteCaregiverAssignmentsAndAppointmentsAsync(assignment);

                string message = $"{{ \"caregiverId\": \"{caregiverId}\", \"message\": \"You are no longer assigned to {patientName}.\" }}";
                await _hubContext.Clients.All
                    .SendAsync("ReceiveAssignmentNotification", message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}