namespace CinemaTicketBooking.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool Status { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
