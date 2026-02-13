using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        // private ApplicationDbContext _context = new();
        //private Repository<Category> _categoryRepository = new();
        //private Repository<Cinema> _cinemaRepository = new();
        //private Repository<Movie> _movieRepository = new();
        //private MovieSubImgRepository _movieSubImgs = new();
        private IRepository<Movie> _movieRepository;
        private IRepository<Category> _categoryRepository ;
        private IRepository<Cinema> _cinemaRepository;
        private IMovieSubImgRepository _movieSubImgs;

        public MovieController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IMovieSubImgRepository movieSubImgs)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieSubImgs = movieSubImgs;
        }

        public async Task<IActionResult> Index(MovieFilterVM filterVM, int page = 1)
        {
            //var movies = _context.Movies.AsNoTracking().AsQueryable();
            var movies = await _movieRepository.GetAsync(includes: [m => m.Category, m => m.Cinema], tracking: false);
            //movies = movies.Include(p => p.Category).Include(p => p.Cinema);

            MovieFilterVM filterVMResponse = new();
            //var categories = _context.Categories.AsNoTracking().AsQueryable();
            var categories = await _categoryRepository.GetAsync(tracking: false);
           // var brands = _context.Cinemas.AsNoTracking().AsQueryable();
            var brands = await _cinemaRepository.GetAsync(tracking: false);
            if (filterVM.Name is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterVM.Name)).ToList();
                filterVMResponse.Name = filterVM.Name;
            }
            if (filterVM.MinPrice is not null)
            {
                movies = movies.Where(e => e.Price >= filterVM.MinPrice).ToList();
                filterVMResponse.MinPrice = filterVM.MinPrice;
            }
            if (filterVM.MaxPrice is not null)
            {
                movies = movies.Where(e => e.Price <= filterVM.MaxPrice).ToList();
                filterVMResponse.MaxPrice = filterVM.MaxPrice;
            }
            if (filterVM.CategoryId is not null)
            {
                movies = movies.Where(e => e.CategoryId == filterVM.CategoryId).ToList();
                filterVMResponse.CategoryId = filterVM.CategoryId;
            }
            if (filterVM.CinemaId is not null)
            {
                movies = movies.Where(e => e.CinemaId == filterVM.CinemaId).ToList();
                filterVMResponse.CinemaId = filterVM.CinemaId;
            }
            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(movies.Count() / (double)pageSize);
            movies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(new MoviesVM
            {
                Movies = movies,
                Categories = categories,
                Cinemas = brands,
                TotalPages = totalCount,
                CurrentPage = currentPage,
                FilterVM = filterVMResponse
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // var categories = _context.Categories.AsNoTracking().AsQueryable();
            var categories = await _categoryRepository.GetAsync(tracking: false);
            //var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            var cinemas = await _cinemaRepository.GetAsync(tracking: false);
            return View(new MovieCreateVM
            {
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieCreateVM movievm , IFormFile mainImg, List<IFormFile> subImgs)
        {

            ModelState.Remove("MainImg");
            ModelState.Remove("Movie.MainImg");
            ModelState.Remove("Movie.Cinema");
            ModelState.Remove("Movie.Category");
            ModelState.Remove("Cinemas");
            ModelState.Remove("Categories");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                // movievm.Categories = _context.Categories.AsNoTracking().AsQueryable();
                movievm.Categories = await _categoryRepository.GetAsync(tracking: false);  
               // movievm.Cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
                movievm.Cinemas = await _cinemaRepository.GetAsync(tracking: false); 
                return View(movievm);
            }

            if (mainImg is not null && mainImg.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(mainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                movievm.Movie.MainImg = newFileName;
            }
    
            //_context.Movies.Add(movievm.Movie);
            //_context.SaveChanges();
            await _movieRepository.CreateAsync(movievm.Movie);
            await _movieRepository.CommitAsync();

            if (subImgs.Any())
            {
                foreach (var img in subImgs)
                {
                    if (img is not null && img.Length > 0)
                    {
                        var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", newFileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            img.CopyTo(stream);
                        }

                        //_context.MovieSubImgs.Add(new()
                        //{
                        //    MovieId = movievm.Movie.Id,
                        //    SubImgs = newFileName
                        //});
                        await _movieSubImgs.CreateAsync(new MovieSubImgs
                        {
                            MovieId = movievm.Movie.Id,
                            SubImgs = newFileName
                        });
                    }
                }
               // _context.SaveChanges();
                await _movieSubImgs.CommitAsync();
                TempData["success-notification"] = "Category created successfully";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var movie = _context.Movies.Find(id);
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie is null)
                return NotFound();
           // var moviesSubImgs = _context.MovieSubImgs.Where(p => p.MovieId == id);
            var moviesSubImgs = await _movieSubImgs.GetAsync(e => e.MovieId == id, tracking: false);
           // var categories = _context.Categories.AsNoTracking().AsQueryable();
            var categories = await _categoryRepository.GetAsync(tracking: false);
            //var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            var cinemas = await _cinemaRepository.GetAsync(tracking: false);
            return View(new MovieUpdateResponseVM
            {
             //Id = movie.Id,
             //   Name = movie.Name,
             //   Description = movie.Description,
             //   Price = movie.Price,
             //   Date = movie.Date,
             //   Status = movie.Status,
             //   MainImg = movie.MainImg,
             //   CategoryId = movie.CategoryId,
             //   CinemaId = movie.CinemaId,
                Movie = movie,
                SubImgs = moviesSubImgs,
                Categories = categories,
                Cinemas = cinemas
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MovieUpdateResponseVM movievm, IFormFile? mainImg, List<IFormFile>? subImgs)
        {
            ModelState.Remove("MainImg");
            ModelState.Remove("Movie.MainImg");
            ModelState.Remove("Movie.Cinema");
            ModelState.Remove("Movie.Category");
            ModelState.Remove("Cinemas");
            ModelState.Remove("Categories");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                //movievm.SubImgs = _context.MovieSubImgs.Where(p => p.MovieId == movievm.Movie.Id);
                movievm.SubImgs = await _movieSubImgs.GetAsync(e => e.MovieId == movievm.Movie.Id, tracking: false);
               // movievm.Categories = _context.Categories.AsNoTracking().AsQueryable();
                movievm.Categories = await _categoryRepository.GetAsync(tracking: false);
               // movievm.Cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
                movievm.Cinemas = await _cinemaRepository.GetAsync(tracking: false);
                return View(movievm);
            }
            //var movieInDB = _context.Movies.AsNoTracking().FirstOrDefault(p => p.Id == movievm.Movie.Id);
                var movieInDB = await _movieRepository.GetOneAsync(p => p.Id == movievm.Movie.Id, tracking: false);
            if (movieInDB is null)
                return NotFound();
            if (mainImg is not null && mainImg.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(mainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                movievm.Movie.MainImg = newFileName;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", movieInDB.MainImg);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                movievm.Movie.MainImg = newFileName;
            }
            else
            {
                movievm.Movie.MainImg = movieInDB.MainImg;
            }
           
            //_context.Movies.Update(movievm.Movie);
            //_context.SaveChanges();
             _movieRepository.Update(movievm.Movie);
                await _movieRepository.CommitAsync();
            if (subImgs.Any())
            {
               // var oldSubImgs = _context.MovieSubImgs.Where(p => p.MovieId == movievm.Movie.Id);
                var oldSubImgs = await _movieSubImgs.GetAsync(e => e.MovieId == movievm.Movie.Id, tracking: false);
                foreach (var img in subImgs)
                {
                    var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", newFileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                    //_context.MovieSubImgs.Add(new()
                    //{
                    //    MovieId = movievm.Movie.Id,
                    //    SubImgs = newFileName
                    //});
                    await _movieSubImgs.CreateAsync(new MovieSubImgs
                    {
                        MovieId = movievm.Movie.Id,
                        SubImgs = newFileName
                    });
                }
                foreach (var oldImg in oldSubImgs)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", oldImg.SubImgs);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                //_context.MovieSubImgs.RemoveRange(oldSubImgs);
                //_context.SaveChanges();
                _movieSubImgs.DeleteRange(oldSubImgs);
                    await _movieSubImgs.CommitAsync();
                TempData["success-notification"] = "Category updated successfully";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteImg([FromRoute] int id, [FromQuery] int ImgId)
        {
            // var subImg = _context.MovieSubImgs.Find(productImgId);
            var movieSubImg = await _movieSubImgs.GetOneAsync(e => e.Id == ImgId);
            if (movieSubImg is null)
                return NotFound();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", movieSubImg.SubImgs);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            //_context.MovieSubImgs.Remove(subImg);
            //_context.SaveChanges();
            _movieSubImgs.Delete(movieSubImg);
            await _movieSubImgs.CommitAsync();
            TempData["success-notification"] = "Category deleted successfully";
            return RedirectToAction(nameof(Edit), new { id });
        }
        public async Task<IActionResult> Delete(int id)
        {
           // var movie = _context.Movies.Find(id);
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie is null)
                return NotFound();
            var mainImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", movie.MainImg);
            if (System.IO.File.Exists(mainImgPath))
            {
                System.IO.File.Delete(mainImgPath);
            }
            //var subImgs = _context.MovieSubImgs.Where(p => p.MovieId == id);
            var subImgs = await _movieSubImgs.GetAsync(e => e.MovieId == id, tracking: false);
            foreach (var subImg in subImgs)
            {
                var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", subImg.SubImgs);
                if (System.IO.File.Exists(subImgPath))
                {
                    System.IO.File.Delete(subImgPath);
                }
            }
            //_context.MovieSubImgs.RemoveRange(subImgs);
            //_context.Movies.Remove(movie);
            //_context.SaveChanges();
            _movieSubImgs.DeleteRange(subImgs);
                _movieRepository.Delete(movie);
            await _movieSubImgs.CommitAsync();
            TempData["success-notification"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
