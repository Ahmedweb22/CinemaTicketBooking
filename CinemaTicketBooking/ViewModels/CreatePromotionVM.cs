namespace CinemaTicketBooking.ViewModels
{
    public class CreatePromotionVM
    {
        public Promotion Promotion { get; set; }
        public IEnumerable<ApplicationUser>? Users { get; set; }=null;
        public IEnumerable<Movie>? Movies { get; set; } = null;

    }
}
