using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class ActorsController : Controller
    {
        //private ApplicationDbContext _context = new();
        //private Repository<Actors> _actorsRepository = new();
        //private Repository<Movie> _movieRepository = new();
        private IRepository<Actors> _actorsRepository;
        private IRepository<Movie> _movieRepository ;

        public ActorsController(IRepository<Actors> actorsRepository, IRepository<Movie> movieRepository)
        {
            _actorsRepository = actorsRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(int? MovieId,string? name, int page = 1)
        {
           // var actors = _context.Actors.AsNoTracking().AsQueryable();
          //  actors = actors.Include(x => x.Movie);
          var actors = await _actorsRepository.GetAsync(includes: [a => a.Movie], tracking: false);

            //var movies = _context.Movies.AsNoTracking().AsQueryable();
            var movies = await _movieRepository.GetAsync(tracking: false);
            if (name is not null)
               actors = actors.Where(e => e.Name.Contains(name)).ToList();
        if (MovieId is not null)
                actors = actors.Where(e => e.MovieId == MovieId).ToList();

            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(actors.Count() / (double)pageSize);
            actors = actors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(new ActorsVM
            {
                Actors = actors.AsEnumerable(),
                Movies = movies.AsEnumerable(),
                CurrentPage = currentPage,
                TotalPages = totalCount
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
           // var movies = _context.Movies.AsNoTracking().AsQueryable();
            var movies = await _movieRepository.GetAsync(tracking: false);
            return View(new ActorsCreateVM
            {
                Movies = movies.AsEnumerable()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(ActorsCreateVM actorvm, IFormFile img)
        {
            ModelState.Remove("Img");
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                // actorvm.Movies = _context.Movies.AsNoTracking().AsQueryable();
                actorvm.Movies = await _movieRepository.GetAsync(tracking: false);
                return View(actorvm);
            }
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                actorvm.Actors.Img = newFileName;
            //_context.Actors.Add(actorvm.Actors);
            //_context.SaveChanges();
            await _actorsRepository.CreateAsync(actorvm.Actors);
            await _actorsRepository.CommitAsync();
            TempData["success-notification"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {

            //var actor = _context.Actors.Find(id);
            var actor = await _actorsRepository.GetOneAsync(e => e.Id == id);

           // var movies = _context.Movies.AsNoTracking().AsQueryable();
           var movies = await _movieRepository.GetAsync(tracking: false);
            if (actor is null)
                return NotFound();
            return View(new ActorsUpdateREsponseVM
            { 
                Id = actor.Id,
                Name = actor.Name,
Description = actor.Description,
Img = actor.Img,
MovieId = actor.MovieId,
                Movies = movies.AsEnumerable()
            });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(ActorsUpdateREsponseVM actorVM, IFormFile? img)
        {
         
            ModelState.Remove("Actors.Movie");
            ModelState.Remove("Img");
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                // actorVM.Movies = _context.Movies;
                actorVM.Movies = await _movieRepository.GetAsync(tracking: false);
                return View(actorVM);
            }
           // var existingActor = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actorVM.Id);
            var existingActor = await _actorsRepository.GetOneAsync(e => e.Id == actorVM.Id, tracking: false);

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
                actorVM.Img = newFileName;
            }
            else
            {
                actorVM.Img = existingActor.Img;
            }
            //existingActor.Name = actorVM.Name;
            //    existingActor.Description = actorVM.Description;
            //    existingActor.MovieId = actorVM.MovieId;

            var actors = new Actors
            {
                Id = actorVM.Id,
                Name = actorVM.Name,
                Description = actorVM.Description,
                Img = actorVM.Img,
                MovieId = actorVM.MovieId,
            };
            //_context.Actors.Update(actors);
            //_context.SaveChanges();
            _actorsRepository.Update(actors);
            await _actorsRepository.CommitAsync();
            TempData["success-notification"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // var actor = _context.Actors.Find(id);
            var actor = await _actorsRepository.GetOneAsync(e => e.Id == id);
            if (actor is null)
                return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\actors_images", actor.Img);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            //_context.Actors.Remove(actor);
            //_context.SaveChanges();
            _actorsRepository.Delete(actor);
            await _actorsRepository.CommitAsync();
            TempData["success-notification"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
