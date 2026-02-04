namespace CinemaTicketBooking.ViewModels
{
    public class CinemasVM
    {
        public IEnumerable<Cinema> Cinemas { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
