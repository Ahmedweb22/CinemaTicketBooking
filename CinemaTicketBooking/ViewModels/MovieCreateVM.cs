namespace CinemaTicketBooking.ViewModels
{
    public class MovieCreateVM
    {
        public Movie Movie { get; set; }
        public IEnumerable<Category> Categories { get; set; } 
        public IEnumerable<Cinema> Cinemas { get; set; } 
    }
}
