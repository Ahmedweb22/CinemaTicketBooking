

namespace CinemaTicketBooking.Repositories
{
    public class MovieSubImgRepository : Repository<MovieSubImgs>, IMovieSubImgRepository
    {
        private ApplicationDbContext _context = new();
        public void DeleteRange(List<MovieSubImgs> movieSubImgs)
            { 
        _context.MovieSubImgs.RemoveRange(movieSubImgs);
        }
    }
}
