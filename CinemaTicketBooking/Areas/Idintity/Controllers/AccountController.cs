using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Cryptography;

namespace CinemaTicketBooking.Areas.Idintity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var user = new ApplicationUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FName = registerVM.FName,
                LName = registerVM.LName,
                Address = registerVM.Address
            };
            //var user = registerVM.Adapt<ApplicationUser>();
            //user.Id = Guid.NewGuid().ToString();
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmE", "Account", new { area = "Identity", token, user.Id },Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"<h1>Please confirm your account by clicking <a href=' {confirmationLink }'>here</a>.</h1>");
            TempData["success-notification"] = "Registration successful!";
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Confirm(string id ,string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["error-notification"] = $"Invalid confirmation link , please try Again";
            }
            else
            {
                TempData["success-notification"] = $"Confirm Account Successfuly";
            }
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {

            if (!ModelState.IsValid)
            {
                return View(loginVM);

            }
            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailOrUserName);
            if (user == null)
            {
                ModelState.AddModelError("EmailOrUserName", "Invalid User Name or Email");
                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginVM);
            }
            // var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("EmailOrUserName", "Confirm your email before logging in.");
                    return View(loginVM);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too Many Attempts, please try again later.");
                    return View(loginVM);
                }
                ModelState.AddModelError("EmailOrUserName", "Invalid User Name or Email");
                ModelState.AddModelError("Password", "Invalid Password");

                return View(loginVM);
            }
            TempData["success-notification"] = $"Welcome back, {user.UserName}";
            return RedirectToAction("Index", "Home" , new {area = "Customer"});
        }
    }
}
