using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Visionmore.Email;
using System;
using System.Threading.Tasks;
using Visionmore.Data;
using Visionmore.Models;

namespace Visionmore.Controllers {
    [AllowAnonymous]
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _db;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext db) {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto) {
            if (!ModelState.IsValid)
                return View();

            User user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user is not null) {
                ViewData["exist"] = "This email is already exist";
                return View(userDto);
            }

            user = new() {
                Name = userDto.FirstName +" " + userDto.LastName,
                UserName = Guid.NewGuid().ToString(),
                Email = userDto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                return View();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmail(user.Email.Trim(), confirmationLink);
            
            if (!emailResponse)
                return View();

            return RedirectToAction("Link");
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto, string ReturnUrl) {
            User user = await _userManager.FindByEmailAsync(userDto.Email);
            bool isHe = await _userManager.CheckPasswordAsync(user, userDto.Password);

            if (!isHe) {
                ViewBag.isHe = "Email or Password is invalid";
                return View();
            }

            bool emailStatus = await _userManager.IsEmailConfirmedAsync(user);
            if (emailStatus == false) {
                ModelState.AddModelError("", "Email is unconfirmed, please confirm it first");
                return View();
            }

            await _signInManager.SignInAsync(user, true);

            if (!string.IsNullOrEmpty(ReturnUrl))
                return LocalRedirect(ReturnUrl);

            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
        
        [HttpGet]
        public IActionResult Link() => View();

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email) {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
    }
}
