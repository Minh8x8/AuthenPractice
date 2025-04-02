using System.Security.Claims;
using AuthenPractice.Models;
using AuthenPractice.Models.Entities;
using AuthenPractice.Repositories;
using AuthenPractice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenPractice.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthController(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _userRepository.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            string salt;
            string hashedPassword = PasswordHelper.HashPassword(model.Password, out salt);

            var user = new User
            {
                Username = model.Email,
                Email = model.Email,
                Password = hashedPassword,
                Salt = salt,
                FirstName = model.FirstName,
                LastName = model.LastName,
                RoleId = new Guid("f63a0ef3-5e85-4764-ae18-3589f8995207")
            };

            await _userRepository.AddAsync(user);

            return RedirectToAction("Login");
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(); // return the login page
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }

            var token = _tokenService.GenerateToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // Important for security: prevents client-side JavaScript from accessing the cookie
                Secure = Request.IsHttps, // Only send over HTTPS in production
                SameSite = SameSiteMode.Strict, // Helps prevent CSRF attacks
                Expires = DateTimeOffset.Now.AddHours(1) // Set an appropriate expiration time
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt"); // Clear the JWT cookie
            return RedirectToAction("Index", "Home"); // Redirect to the home page after logout
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Role = user.Role.Name
            });
        }
    }
}
