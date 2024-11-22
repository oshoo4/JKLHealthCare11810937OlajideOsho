using JKLHealthCare11810937.Models.DTOs;
using JKLHealthCare11810937.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using JKLHealthCare11810937.Services.Security;
using JKLHealthCare11810937.Services.Repository;
using Microsoft.AspNetCore.Authorization;

namespace JKLHealthCare11810937.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IValidationService _validationService;

        public AccountController(
            IRepository repository,
            IUserAuthenticationService userAuthenticationService,
            IValidationService validationService
        )
        {
            _repository = repository;
            _userAuthenticationService = userAuthenticationService;
            _validationService = validationService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (registerDto.Username.Length < 6 || registerDto.Username.Length > 20)
            {
                ModelState.AddModelError("Username", "Username must be between 6 and 20 characters long.");
            }
            if (registerDto.PasswordHash.Length > 0 && !_validationService.IsPasswordComplex(registerDto.PasswordHash))
            {
                ModelState.AddModelError(
                    "PasswordHash",
                    "Password must be at least 8 characters long and contain lowercase, uppercase, digits, and special characters."
                );
            }
            if (ModelState.IsValid)
            {
                var hashedPassword = _userAuthenticationService.HashPassword(registerDto.PasswordHash);

                var user = new User
                {
                    Username = registerDto.Username,
                    Role = registerDto.Role,
                    PasswordHash = hashedPassword
                };

                await _repository.AddUserAsync(user);
                return RedirectToAction("Index", "Home");
            }
            return View(registerDto);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {

            if (ModelState.IsValid)
            {
                var dbUser = await _repository.GetUserByUsernameAsync(loginDto.Username);

                if (
                    dbUser != null
                    && _userAuthenticationService.VerifyPassword(loginDto.PasswordHash, dbUser.PasswordHash)
                )
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, dbUser.Username),
                        new Claim(ClaimTypes.Role, dbUser.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                    await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(claimsIdentity));

                    HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                    HttpContext.Session.SetString("UserRole", dbUser.Role);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            return View(loginDto);
        }

        [Authorize]
        public IActionResult GetUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            return Json(userId);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}