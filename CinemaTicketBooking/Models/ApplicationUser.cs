using Microsoft.AspNetCore.Identity;

namespace CinemaTicketBooking.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string? Address { get; set; }
    }
}
