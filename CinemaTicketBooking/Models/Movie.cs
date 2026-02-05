namespace CinemaTicketBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, 1000)]
        public decimal Price { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }
        public bool Status { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public List<Actors> Actors { get; set; } = new List<Actors>();
        public int CategoryId { get; set; }
        
        public Category Category { get; set; }
        public int CinemaId { get; set; }
  
        public Cinema Cinema { get; set; }


    }
}
