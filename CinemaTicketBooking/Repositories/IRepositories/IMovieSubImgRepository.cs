namespace CinemaTicketBooking.Repositories.IRepositories
{
    public interface IMovieSubImgRepository : IRepository<MovieSubImgs>
    {
        void DeleteRange(List<MovieSubImgs> movieSubImgs);
    }
}
