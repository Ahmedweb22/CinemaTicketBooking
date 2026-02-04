namespace CinemaTicketBooking.ViewModels
{
    public class CategoriesVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
