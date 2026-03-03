namespace CinemaTicketBooking.ViewModels
{
    public class PromotionsVM
    {
        public IEnumerable<Promotion> Promotions { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
