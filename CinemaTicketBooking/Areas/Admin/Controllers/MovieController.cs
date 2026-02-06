using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(MovieFilterVM filterVM, int page = 1)
        {
            var movies = _context.Movies.AsNoTracking().AsQueryable();
            movies = movies.Include(p => p.Category).Include(p => p.Cinema);
            MovieFilterVM filterVMResponse = new();
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Cinemas.AsNoTracking().AsQueryable();
            if (filterVM.Name is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterVM.Name));
                filterVMResponse.Name = filterVM.Name;
            }
            if (filterVM.MinPrice is not null)
            {
                movies = movies.Where(e => e.Price >= filterVM.MinPrice);
                filterVMResponse.MinPrice = filterVM.MinPrice;
            }
            if (filterVM.MaxPrice is not null)
            {
                movies = movies.Where(e => e.Price <= filterVM.MaxPrice);
                filterVMResponse.MaxPrice = filterVM.MaxPrice;
            }
            if (filterVM.CategoryId is not null)
            {
                movies = movies.Where(e => e.CategoryId == filterVM.CategoryId);
                filterVMResponse.CategoryId = filterVM.CategoryId;
            }
            if (filterVM.CinemaId is not null)
            {
                movies = movies.Where(e => e.CinemaId == filterVM.CinemaId);
                filterVMResponse.CinemaId = filterVM.CinemaId;
            }
            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(movies.Count() / (double)pageSize);
            movies = movies.Skip((page - 1) * pageSize).Take(pageSize);
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
        public IActionResult Create()
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            return View(new MovieCreateVM
            {
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable()
            });
        }
        [HttpPost]
        public IActionResult Create(MovieCreateVM movie , IFormFile mainImg, List<IFormFile> subImgs)
        {
            if (!ModelState.IsValid)
            { 
               movie.Categories = _context.Categories.AsNoTracking().AsQueryable();
               movie.Cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
                return View(movie);
            }

            if (mainImg is not null && mainImg.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(mainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                movie.MainImg = newFileName;
            }
            var mv = new Movie
            {
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Date = movie.Date,
                MainImg = movie.MainImg,
                Status = movie.Status,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId
            };
            _context.Movies.Add(mv);
            _context.SaveChanges();
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

                        _context.MovieSubImgs.Add(new()
                        {
                            MovieId = mv.Id,
                            SubImgs = newFileName
                        });
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .FirstOrDefault(m =>m.Id==id);
            if (movie is null)
                return NotFound();
            var productSubImgs = _context.MovieSubImgs.Where(p => p.MovieId == id);
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            return View(new MovieUpdateResponseVM
            {
             Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Date = movie.Date,
                Status = movie.Status,
                MainImg = movie.MainImg,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                SubImgs = productSubImgs,
                Categories = categories,
                Cinemas = cinemas
            });
        }
        [HttpPost]
        public IActionResult Edit(MovieUpdateResponseVM movie, IFormFile? mainImg, List<IFormFile>? subImgs)
        {
            ModelState.Remove("MainImg");
            ModelState.Remove("Movies.Cinema");
            ModelState.Remove("Movies.Category");
            ModelState.Remove("Cinemas");
            ModelState.Remove("Categories");
            if (!ModelState.IsValid)
                return View(movie);
            var movieInDB = _context.Movies.AsNoTracking().FirstOrDefault(p => p.Id == movie.Id);
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
                movie.MainImg = newFileName;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", movieInDB.MainImg);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                movie.MainImg = newFileName;
            }
            else
            {
                movie.MainImg = movieInDB.MainImg;
            }
            var mv = new Movie
            {
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Date = movie.Date,
                MainImg = movie.MainImg,
                Status = movie.Status,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId
            };
            _context.Movies.Update(mv);
            _context.SaveChanges();
            if (subImgs.Any())
            {
                var oldSubImgs = _context.MovieSubImgs.Where(p => p.MovieId == movie.Id);
                foreach (var img in subImgs)
                {
                    var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", newFileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                    _context.MovieSubImgs.Add(new()
                    {
                        MovieId = movie.Id,
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
                _context.MovieSubImgs.RemoveRange(oldSubImgs);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteImg([FromRoute] int id, [FromQuery] int productImgId)
        {
            var subImg = _context.MovieSubImgs.Find(productImgId);
            if (subImg is null)
                return NotFound();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", subImg.SubImgs);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.MovieSubImgs.Remove(subImg);
            _context.SaveChanges();
            return RedirectToAction(nameof(Edit), new { id });
        }
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie is null)
                return NotFound();
            var mainImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images", movie.MainImg);
            if (System.IO.File.Exists(mainImgPath))
            {
                System.IO.File.Delete(mainImgPath);
            }
            var subImgs = _context.MovieSubImgs.Where(p => p.MovieId == id);
            foreach (var subImg in subImgs)
            {
                var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\movie_images\\sub_images", subImg.SubImgs);
                if (System.IO.File.Exists(subImgPath))
                {
                    System.IO.File.Delete(subImgPath);
                }
            }
            _context.MovieSubImgs.RemoveRange(subImgs);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
