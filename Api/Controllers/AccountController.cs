using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels.Identities;
using reCAPTCHA.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole<int>> _roleManager;

        private readonly IRecaptchaService _recaptcha;

        public IdentityController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager, IRecaptchaService recaptcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _recaptcha = recaptcha;
        }

        /// <summary>
        ///     View page to login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public async Task<IActionResult> Login()
        {
            if (TempData.ContainsKey("Error"))
            {
                var prevError = (string) TempData["Error"];

                ModelState.AddModelError("", prevError);

                TempData.Clear();
            }

            return View();
        }

        /// <summary>
        ///     Handles login the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoginHandler")]
        [SwaggerOperation("LoginHandler")]
        public async Task<IActionResult> LoginHandler(LoginViewModel loginViewModel)
        {
            var recaptcha = await _recaptcha.Validate(Request);

            if (!recaptcha.success)
            {
                TempData["Error"] = "There was an error validating recatpcha. Please try again!";

                return RedirectToAction("Login");
            }

            // Ensure the username and password is valid.
            var result = await _userManager.FindByNameAsync(loginViewModel.Username);

            if (result == null || !await _userManager.CheckPasswordAsync(result, loginViewModel.Password))
            {
                TempData["Error"] = "There was an error while logging-in. Please try again!";

                return RedirectToAction("Login");
            }

            await _signInManager.SignInAsync(result, true);

            // Generate and issue a JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.UserName),
                new Claim(ClaimTypes.Email, result.Email)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal), authProperties);
            
            return Redirect(Url.Content("~/"));
        }

        /// <summary>
        ///     View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> Register()
        {
            if (TempData.ContainsKey("Error"))
            {
                var prevError = (string) TempData["Error"];

                ModelState.AddModelError("", prevError);

                TempData.Clear();
            }

            return View();
        }

        /// <summary>
        ///     Register the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterHandler")]
        [SwaggerOperation("RegisterHandler")]
        public async Task<IActionResult> RegisterHandler(RegisterViewModel registerViewModel)
        {
            var recaptcha = await _recaptcha.Validate(Request);

            if (!recaptcha.success)
            {
                TempData["Error"] = "There was an error validating recatpcha. Please try again!";

                return RedirectToAction("Register");
            }

            var user = new User
            {
                UserName = registerViewModel.Username,
                Name = registerViewModel.Name,
                Email = registerViewModel.Email
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = "There was an error while registering. Please try again!";

                return RedirectToAction("Register");
            }
            
            return RedirectToAction("Login");
        }

        /// <summary>
        ///     Not authenticated view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NotAuthenticated")]
        [SwaggerOperation("NotAuthenticated")]
        public async Task<IActionResult> NotAuthenticated()
        {
            return View();
        }

        /// <summary>
        ///     Not authorized view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}