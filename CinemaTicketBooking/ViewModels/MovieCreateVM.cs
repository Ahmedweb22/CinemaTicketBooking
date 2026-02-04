namespace CinemaTicketBooking.ViewModels
{
    public class MovieCreateVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string? MainImg { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }


        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
    }
}
