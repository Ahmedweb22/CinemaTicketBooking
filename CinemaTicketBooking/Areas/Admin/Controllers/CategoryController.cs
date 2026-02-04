using CinemaTicketBooking.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(string? name, int page = 1)
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            if (name is not null)
                categories = categories.Where(e => e.Name.Contains(name));
            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(categories.Count() / (double)pageSize);
            categories = categories.Skip((page - 1) * pageSize).Take(pageSize);
            return View(new CategoriesVM
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalCount,
                CurrentPage = currentPage
            });

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (!ModelState.IsValid)
                return View(category);
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null)
                return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
