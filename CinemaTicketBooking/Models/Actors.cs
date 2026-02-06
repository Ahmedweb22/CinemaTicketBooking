namespace CinemaTicketBooking.Models
{
    public class Actors
    {
        public int Id { get; set; }
        [Required]
        [Length(3, 255)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Length(3, 500)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Img { get; set; } = null!;
        [Required]
        public int MovieId { get; set; }

        public Movie Movie { get; set; } 

    }
}
