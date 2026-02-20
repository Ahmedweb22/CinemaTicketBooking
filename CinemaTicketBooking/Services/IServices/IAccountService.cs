using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Services.IServices
{
    public interface IAccountService
    {
        Task SendEmailAsync(EmailType emailType, string msg, ApplicationUser user);
    }
}
