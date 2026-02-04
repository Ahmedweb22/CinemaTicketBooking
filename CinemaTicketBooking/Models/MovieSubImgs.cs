namespace CinemaTicketBooking.Models
{
    public class MovieSubImgs
    {
        public int Id { get; set; }
        public string SubImgs { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public Movie Movie { get; set; }=default!;
    }
}
