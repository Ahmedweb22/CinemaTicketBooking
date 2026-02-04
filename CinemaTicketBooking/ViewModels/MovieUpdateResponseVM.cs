namespace CinemaTicketBooking.ViewModels
{
    public class MovieUpdateResponseVM
    {
        public Movie Movies { get; set; }= null!;
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public IEnumerable<MovieSubImgs> SubImgs { get; set; } = null!;
    }
}
