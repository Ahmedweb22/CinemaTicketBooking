using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ActorsController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(int? MovieId,string? name, int page = 1)
        {
            var actors = _context.Actors.AsNoTracking().AsQueryable();
            actors = actors.Include(x => x.Movie);

            var movies = _context.Movies.AsNoTracking().AsQueryable();
            if (name is not null)
                actors = actors.Where(e => e.Name.Contains(name));
        if (MovieId is not null)
                actors = actors.Where(e => e.MovieId == MovieId);

            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(actors.Count() / (double)pageSize);
            actors = actors.Skip((page - 1) * pageSize).Take(pageSize);

            return View(new ActorsVM
            {
                Actors = actors.AsEnumerable(),
                Movies = movies.AsEnumerable(),
                CurrentPage = currentPage,
                TotalPages = totalCount
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actors());
        }
        [HttpPost]
        public IActionResult Create(Actors actor, IFormFile img)
        {
            if (!ModelState.IsValid)
                return View(actor);
            if (img is not null && img.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                actor.Img = newFileName;
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
                return NotFound();
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actors actor, IFormFile? img)
        {
            if (!ModelState.IsValid)
                return View(actor);
            Actors? existingActor = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actor.Id);
            if (existingActor is null)
                return NotFound();
            if (img is not null && img.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", existingActor.Img);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                actor.Img = newFileName;
            }
            else
            {
                actor.Img = existingActor.Img;
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
                return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", actor.Img);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
