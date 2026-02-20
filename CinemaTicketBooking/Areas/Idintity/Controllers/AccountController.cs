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
        private readonly IAccountService _accountService;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender ,
            IAccountService accountService, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _accountService = accountService;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
             TempData["success-notification"] = $"Logout Successfully";
             return RedirectToAction("Login", "Account", new { area = "Identity" });
           
        }
        [HttpGet]
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
            var confirmationLink = Url.Action("ConfirmE", "Account", new { area = "Identity", token, user.Id }, Request.Scheme);
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            //    $"<h1>Please confirm your account by clicking <a href=' {confirmationLink }'>here</a>.</h1>");

            await _accountService.SendEmailAsync(EmailType.Confirmation, $"<h1>Please confirm your account by clicking <a href=' {confirmationLink}'>here</a>.</h1>", user);

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
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {

            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);

            }
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);
            if (user is not null && !user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmE", "Account", new { area = "Identity", token, user.Id }, Request.Scheme);
                     await _accountService.SendEmailAsync(EmailType.ResendConfirmation, $"<h1>Please confirm your account by clicking <a href=' {confirmationLink}'>here</a>.</h1>", user);
            }
            TempData["success-notification"] = $"Resend Email Successfully, if account is exist and not confirmed";
        
                return RedirectToAction("Login", "Account", new { area = "Identity" });
       
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {

            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);

            }
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);
            var userOtpsCount = (await _applicationUserOTPRepository.GetAsync(e =>user.Id == e.ApplicationUserId && e.CreateAt >= DateTime.UtcNow.AddHours(-24))).Count();
            if (!user.EmailConfirmed)
            {
                return RedirectToAction("ResendEmailConfirmation", "Account", new { area = "Identity"});
            } 
            if (user is not null && userOtpsCount <= 5)
            {
                string otp = new Random().Next(1000, 9999).ToString();
                string msg = $"<h1>Your OTP for resetting password is: {otp}</h1>";
                await _accountService.SendEmailAsync(EmailType.ForgetPassword, msg, user);

              await  _applicationUserOTPRepository.CreateAsync(new ApplicationUserOTP
                {
                    ApplicationUserId = user.Id,
                    OTP = otp,
               
                });
                await _applicationUserOTPRepository.CommitAsync();
            
            TempData["success-notification"] = $"Send OTP Successfully";
            }
            else if (userOtpsCount > 5)
            {
                TempData["error-notification"] = $"You have exceeded the maximum number of OTP requests. Please try again later.";
            }

            return RedirectToAction("ValidateOTP", "Account", new { area = "Identity" , userId = user.Id });

        }
        [HttpGet]
        public IActionResult ValidateOTP(string userId)
        {
            return View( new ValidateOTPVM
            {
                ApplicationUserId = userId
            });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
            {
                return View(validateOTPVM);
            }
            var user= await _userManager.FindByIdAsync(validateOTPVM.ApplicationUserId);
            if (user is null) 
                return NotFound();
            var userOTP = (await _applicationUserOTPRepository.GetAsync()).Where(e => e.ApplicationUserId == user.Id && e.IsValid)
                .OrderBy(e=>e.Id).LastOrDefault();
            if (userOTP == null)
                {
                TempData["error-notification"] = $"Invalid OTP, please try again.";
                return View(validateOTPVM);
            }
           
             userOTP.IsUsed = true;
                return RedirectToAction("ResetPassword", "Account", new { area = "Identity", userId = user.Id });
            
        
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId)
        {
            return View(new ResetPasswordVM
            {
                ApplicationUserId = userId
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);
            if (user is null)
                return NotFound();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetPasswordVM);
            }
            TempData["success-notification"] = $"Password reset successfully";
            return RedirectToAction("Login", "Account", new { area = "Identity"});
        }
      
    }
    }

