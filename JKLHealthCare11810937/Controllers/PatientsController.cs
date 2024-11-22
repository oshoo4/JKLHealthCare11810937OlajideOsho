using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JKLHealthCare11810937.Services.Security;
using JKLHealthCare11810937.Services.Repository;

namespace JKLHealthCare11810937.Controllers
{
    [Authorize(Roles = "administrator")]
    public class PatientsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IEncryptionService _encryptionService;

        public PatientsController(
            IRepository repository,
            IEncryptionService encryptionService
        )
        {
            _repository = repository;
            _encryptionService = encryptionService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllPatientsAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            var allPatients = await _repository.GetAllPatientsAsync();
            if (id == null || allPatients.Count == 0)
            {
                return NotFound();
            }

            var patient = await _repository.GetPatientByIdAsync((int)id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.MedicalRecords = _encryptionService.Decrypt(patient.MedicalRecords);
            return View(patient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientDTO patientDto)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    Name = patientDto.Name,
                    Address = patientDto.Address,
                    MedicalRecords = _encryptionService.Encrypt(patientDto.MedicalRecords)
                };

                await _repository.AddPatientAsync(patient);
                return RedirectToAction(nameof(Index));
            }
            return View(patientDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var allPatients = await _repository.GetAllPatientsAsync();
            if (id == null || allPatients.Count == 0)
            {
                return NotFound();
            }

            var patient = await _repository.GetPatientByIdAsync((int)id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.MedicalRecords = _encryptionService.Decrypt(patient.MedicalRecords);

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("PatientId,Name,Address,MedicalRecords")] Patient patient
        )
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    patient.MedicalRecords = _encryptionService.Encrypt(patient.MedicalRecords);

                    await _repository.UpdatePatientAsync(patient);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.PatientExists(patient))
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
            return View(patient);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var allPatients = await _repository.GetAllPatientsAsync();
            if (id == null || allPatients.Count == 0)
            {
                return NotFound();
            }

            var patient = await _repository.GetPatientByIdAsync((int)id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.MedicalRecords = _encryptionService.Decrypt(patient.MedicalRecords);
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allPatients = await _repository.GetAllPatientsAsync();
            if (allPatients.Count == 0)
            {
                return Problem("Entity set 'JKLHealthCareContext.Patients' is null.");
            }
            var patient = await _repository.GetPatientByIdAsync(id);
            if (patient != null)
            {
                await _repository.DeletePatientWithAssignmentsAndAppointmentsAsync(id, patient);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
