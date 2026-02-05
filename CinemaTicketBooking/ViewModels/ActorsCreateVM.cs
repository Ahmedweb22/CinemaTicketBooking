namespace CinemaTicketBooking.ViewModels
{
    public class ActorsCreateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public int MovieId { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}
