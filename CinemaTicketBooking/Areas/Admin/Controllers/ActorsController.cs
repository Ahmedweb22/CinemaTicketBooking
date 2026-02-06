using System.Numerics;
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
            var movies = _context.Movies.AsNoTracking().AsQueryable();
            return View(new ActorsCreateVM
            {
                Movies = movies.AsEnumerable()
            });
        }
        [HttpPost]
        public IActionResult Create(ActorsCreateVM actor, IFormFile img)
        {
            ModelState.Remove("Img");
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                actor.Movies = _context.Movies.AsNoTracking().AsQueryable();
                return View(actor);
            }
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
            
            var actors = new Actors
            {
                Name = actor.Name,
                Description = actor.Description,
                Img = newFileName,
                MovieId = actor.MovieId,
            };
            _context.Actors.Add(actors);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {

            var actor = _context.Actors.Find(id);
            var movies = _context.Movies.AsNoTracking().AsQueryable();
            if (actor is null)
                return NotFound();
            return View(new ActorsUpdateREsponseVM
            { 
Name = actor.Name,
Description = actor.Description,
Img = actor.Img,
MovieId = actor.MovieId,
                Movies = movies.AsEnumerable()
            });
        }
        [HttpPost]
        public IActionResult Edit(ActorsUpdateREsponseVM actorVM, IFormFile? img)
        {
            ModelState.Remove("Actors.Movie");
            ModelState.Remove("Img");
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            { 
                actorVM.Movies = _context.Movies;
                return View(actorVM);
            }
            var existingActor = _context.Actors.FirstOrDefault(b => b.Id == actorVM.Id);
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
                if (!string.IsNullOrEmpty(existingActor.Img))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", existingActor.Img);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                actorVM.Img = newFileName;
            }
            existingActor.Name = actorVM.Name;
                existingActor.Description = actorVM.Description;
                existingActor.MovieId = actorVM.MovieId;
           
           // _context.Actors.Update(actors);
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
