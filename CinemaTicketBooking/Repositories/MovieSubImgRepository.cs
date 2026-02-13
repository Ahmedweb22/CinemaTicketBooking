

namespace CinemaTicketBooking.Repositories
{
    public class MovieSubImgRepository : Repository<MovieSubImgs>, IMovieSubImgRepository
    {
        private ApplicationDbContext _context;

        public MovieSubImgRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void DeleteRange(List<MovieSubImgs> movieSubImgs)
            { 
        _context.MovieSubImgs.RemoveRange(movieSubImgs);
        }
    }
}
