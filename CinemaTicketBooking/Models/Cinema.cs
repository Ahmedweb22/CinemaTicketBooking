namespace CinemaTicketBooking.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
