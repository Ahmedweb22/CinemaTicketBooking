namespace CinemaTicketBooking.Models
{
    public class Statistics
    {
        public int Id { get; set; }
        [Required]
        public int TotalMovies { get; set; }
        [Required]
        public int TotalCategories { get; set; }
        [Required]
        public int TotalCinemas { get; set; }
        [Required]
        public int TotalActors { get; set; }
        [Required]
        public decimal TotalRevenue { get; set; }
    }
}
