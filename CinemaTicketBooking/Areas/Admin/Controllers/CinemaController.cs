using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        //private ApplicationDbContext _context = new();
                private Repository<Cinema> _cinemaRepository = new();
        public async Task<IActionResult> Index(string? name, int page = 1)
        {

            //var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            var cinemas =await _cinemaRepository.GetAsync(tracking:false);
            if (name is not null)
                cinemas = cinemas.Where(e => e.Name.Contains(name)).ToList();
            if (page < 1)
                page = 1;
            int pageSize = 5;
            int currentPage = page;
            double totalCount = Math.Ceiling(cinemas.Count() / (double)pageSize);
            cinemas = cinemas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> Create(Cinema cinema, IFormFile img)
        {
            ModelState.Remove("Img");
            if (!ModelState.IsValid)
                return View(cinema);
            if (img is not null && img.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                cinema.Img = newFileName;
            }

            //_context.Cinemas.Add(cinema);
            //_context.SaveChanges();
            await _cinemaRepository.CreateAsync(cinema);
            await _cinemaRepository.CommitAsync();  
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
           
            //var cinema = _context.Cinemas.Find(id);
            var cinema = await _cinemaRepository.GetOneAsync(b => b.Id == id);
            if (cinema is null)
                return NotFound();
            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? img
            )
        {
            ModelState.Remove("Img");
            if (!ModelState.IsValid)
                return View(cinema);
            //var existingCinema = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);
            var existingCinema = await _cinemaRepository.GetOneAsync(b => b.Id == cinema.Id, tracking: false);
            if (existingCinema is null)
                return NotFound();
            if (img is not null && img.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
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
            //_context.Cinemas.Update(cinema);
            //_context.SaveChanges();
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //var cinema = _context.Cinemas.Find(id);
                var cinema = await _cinemaRepository.GetOneAsync(b => b.Id == id);
            if (cinema is null)
                return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\cinema_posters", cinema.Img);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            //_context.Cinemas.Remove(cinema);
            //_context.SaveChanges();
                _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
