namespace CinemaTicketBooking.ViewModels
{
    public class ActorsUpdateREsponseVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Img { get; set; } = null!;
        public int MovieId { get; set; }
        public IEnumerable<Movie> Movies { get; set; } 
    }
}
