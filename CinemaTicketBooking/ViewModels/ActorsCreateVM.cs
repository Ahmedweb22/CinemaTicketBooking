namespace CinemaTicketBooking.ViewModels
{
    public class ActorsCreateVM
    {
        //public int Id { get; set; }
        //public string Name { get; set; } =null!;
        //public string Description { get; set; } =null!;
        //public string Img { get; set; } =null!;
        //[Required]
        //public int MovieId { get; set; }
        public Actors Actors { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}
