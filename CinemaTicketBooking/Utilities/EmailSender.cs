using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CinemaTicketBooking.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("ahmedwebdevelop2@gmail.com" , "hlej rleg bfak ppiy")
            };
            return client.SendMailAsync(
                new MailMessage(from: "ahmedwebdevelop2@gmail.com",
                to: email,
                subject,
                htmlMessage
                )
                {
                    IsBodyHtml = true
                });
        }
    }
}
