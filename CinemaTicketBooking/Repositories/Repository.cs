using System.Linq.Expressions;

namespace CinemaTicketBooking.Repositories
{
    public class Repository<T> where T : class
    {
        private ApplicationDbContext _context = new();
        private DbSet<T> _dbSet;
        public Repository() 
        {
        _dbSet = _context.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null , bool tracking = true)
        {
           
            var categories = _dbSet.AsQueryable();
            if (expression is not null)
            {
                categories = categories.Where(expression);
            }
            if (!tracking)
            {
                categories = categories.AsNoTracking();
            }
            return await categories.ToListAsync();
        }
        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, bool tracking = true)
        {

            return (await GetAsync(expression, tracking)).FirstOrDefault();
        }
      
        public async Task<int> CommitAsync()
        {
            try
            {
            return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while saving changes: {ex.Message}");
                return 0;
            }
        }
    }
}
