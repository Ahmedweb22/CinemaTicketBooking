using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(string? name, int page = 1)
        {

            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            if (name is not null)
                cinemas = cinemas.Where(e => e.Name.Contains(name));
            if (page < 1)
                page = 1;
            int pageSize = 5;
            int currentPage = page;
            double totalCount = Math.Ceiling(cinemas.Count() / (double)pageSize);
            cinemas = cinemas.Skip((page - 1) * pageSize).Take(pageSize);

            return View(new CinemasVM
            {
                Cinemas = cinemas.AsEnumerable(),
                CurrentPage = currentPage,
                TotalPages = totalCount
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cinema());
        }
        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile poster)
        {
            ModelState.Remove("Poster");
            if (!ModelState.IsValid)
                return View(cinema);
            if (poster is not null && poster.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(poster.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    poster.CopyTo(stream);
                }
                cinema.Img = newFileName;
            }

            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
           
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
                return NotFound();
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? poster)
        {
            ModelState.Remove("Img");
            if (!ModelState.IsValid)
                return View(cinema);
            Cinema? existingCinema = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);
            if (existingCinema is null)
                return NotFound();
            if (poster is not null && poster.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(poster.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    poster.CopyTo(stream);
                }
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", existingCinema.Img);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                cinema.Img = newFileName;
            }
            else
            {
                cinema.Img = existingCinema.Img;
            }
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
                return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", cinema.Img);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
