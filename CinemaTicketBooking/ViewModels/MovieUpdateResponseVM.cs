namespace CinemaTicketBooking.ViewModels
{
    public class MovieUpdateResponseVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        public string MainImg { get; set; } 
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public IEnumerable<MovieSubImgs> SubImgs { get; set; } = null!;
    }
}
