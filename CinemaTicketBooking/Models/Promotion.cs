namespace CinemaTicketBooking.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }
        public int MaxUsage { get; set; } = 1;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(15);
        public bool IsValid => MaxUsage >= 1 && ExpiredAt > DateTime.UtcNow;
    }
}
