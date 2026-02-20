using System.Security.Policy;
using Azure.Core;
using CinemaTicketBooking.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Services
{
    public enum EmailType
    {
        Confirmation,
        ResendConfirmation,
        ForgetPassword
    }
    public class AccountService : IAccountService
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(IEmailSender emailSender , UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }
      public async Task  SendEmailAsync(EmailType emailType, string msg, ApplicationUser user  )
        {
        
        
            if (emailType == EmailType.Confirmation )
            {
                await _emailSender.SendEmailAsync(user.Email!, "Confirm your Account!", msg);
            }
            else if (emailType == EmailType.ResendConfirmation)
            {
                await _emailSender.SendEmailAsync(user.Email!, "Resend Confirm your Account!", msg);
            }
            else if (emailType == EmailType.ForgetPassword)
            {
                await _emailSender.SendEmailAsync(user.Email!, "Forget Password!", msg);
            }
        }
    }
}
