using JKLHealthCare11810937.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JKLHealthCare11810937.Models.DTOs;
using JKLHealthCare11810937.Services.Security;
using JKLHealthCare11810937.Services.Repository;

namespace JKLHealthCare11810937.Controllers
{
    [Authorize(Roles = "administrator")]
    public class CaregiversController : Controller
    {
        private readonly IRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IValidationService _validationService;

        public CaregiversController(
            IRepository repository,
            IUserAuthenticationService userAuthenticationService,
            IValidationService validationService
        )
        {
            _repository = repository;
            _userAuthenticationService = userAuthenticationService;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllCaregiversAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            if (id == null || allCaregivers.Count == 0)
            {
                return NotFound();
            }

            var caregiver = await _repository.GetCaregiverByIdAsync((int)id);
            if (caregiver == null)
            {
                return NotFound();
            }

            return View(caregiver);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaregiverId,Name,Contact,Qualifications,Availability,Username,PasswordHash")] CaregiverDTO caregiverDto)
        {
            if (!_validationService.IsValidContact(caregiverDto.Contact))
            {
                ModelState.AddModelError(
                    "Contact",
                    "Please enter a valid email address or phone number."
                );
            }
            if (caregiverDto.Username.Length < 6 || caregiverDto.Username.Length > 20)
            {
                ModelState.AddModelError("Username", "Username must be between 6 and 20 characters long.");
            }
            if (caregiverDto.PasswordHash.Length > 0 && !_validationService.IsPasswordComplex(caregiverDto.PasswordHash))
            {
                ModelState.AddModelError(
                    "PasswordHash",
                    "Password must be at least 8 characters long and contain lowercase, uppercase, digits, and special characters."
                );
            }
            if (ModelState.IsValid)
            {
                var hashedPassword = _userAuthenticationService.HashPassword(caregiverDto.PasswordHash);

                var user = new User
                {
                    Username = caregiverDto.Username,
                    Role = "caregiver",
                    PasswordHash = hashedPassword
                };

                await _repository.AddUserAsync(user);

                var caregiver = new Caregiver
                {
                    Name = caregiverDto.Name,
                    Contact = caregiverDto.Contact,
                    Qualifications = caregiverDto.Qualifications,
                    Availability = caregiverDto.Availability,
                    CaregiverId = user.UserId
                };

                await _repository.AddCaregiverAsync(caregiver);
                return RedirectToAction(nameof(Index));
            }

            return View(caregiverDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            if (id == null || allCaregivers.Count == 0)
            {
                return NotFound();
            }

            var caregiver = await _repository.GetCaregiverByIdAsync((int)id);
            if (caregiver == null)
            {
                return NotFound();
            }
            return View(caregiver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaregiverId,Name,Contact,Qualifications,Availability")] Caregiver caregiver)
        {
            if (id != caregiver.CaregiverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateCaregiverAsync(caregiver);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.CaregiverExists(caregiver.CaregiverId))
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
            return View(caregiver);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            if (id == null || allCaregivers.Count == 0)
            {
                return NotFound();
            }

            var caregiver = await _repository.GetCaregiverByIdAsync((int)id);
            if (caregiver == null)
            {
                return NotFound();
            }

            return View(caregiver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allCaregivers = await _repository.GetAllCaregiversAsync();
            if (allCaregivers.Count == 0)
            {
                return Problem("Entity set 'JKLHealthCareContext.Caregivers'  is null.");
            }
            var caregiver = await _repository.GetCaregiverByIdAsync(id);
            var caregiverUser = await _repository.GetUserById(id);

            if (caregiver != null && caregiverUser != null)
            {
                await _repository.DeleteCaregiverWithAssignmentsAndAppointmentsAsync(id, caregiver, caregiverUser);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}