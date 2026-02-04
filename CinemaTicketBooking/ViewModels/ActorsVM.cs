namespace CinemaTicketBooking.ViewModels
{
    public class ActorsVM
    {
        public IEnumerable<Actors> Actors { get; set; } 
        public IEnumerable<Movie> Movies { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
