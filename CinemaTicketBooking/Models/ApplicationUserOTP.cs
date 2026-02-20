namespace CinemaTicketBooking.Models
{
    public class ApplicationUserOTP
    {
        public int Id  { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime. UtcNow;
        public bool IsUsed { get; set; }
        public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public bool IsValid => ExpireAt > DateTime.UtcNow && !IsUsed;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; }= null!;
    }
}
