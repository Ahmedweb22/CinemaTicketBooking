namespace CinemaTicketBooking.ViewModels
{
    public class MovieFilterVM
    {
        public string? Name { get; set; }
        public long? MinPrice { get; set; }
        public long? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public int? CinemaId { get; set; }
    }
}
