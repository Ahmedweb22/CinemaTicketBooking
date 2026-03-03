namespace CinemaTicketBooking.ViewModels
{
    public class PromotionUpdateResponceVM
    {
        public Promotion promotion {  get; set; } = null!;
        public IEnumerable<Movie>? movies { get; set; }
        public IEnumerable<ApplicationUser>? Users { get; set; }
    }
}
