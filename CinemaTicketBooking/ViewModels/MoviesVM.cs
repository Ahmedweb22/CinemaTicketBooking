namespace CinemaTicketBooking.ViewModels
{
    public class MoviesVM
    {
        public IEnumerable<Movie> Movies { get; set; } 
        public IEnumerable<Category> Categories { get; set; } 
        public IEnumerable<Cinema> Cinemas { get; set; } 
        public MovieFilterVM FilterVM { get; set; } 
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
