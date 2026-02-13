using CinemaTicketBooking.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        //private ApplicationDbContext _context = new();
        //private Repository<Category> _categoryRepository = new();
        private IRepository<Category> _categoryRepository;
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(string? name, int page = 1)
        {
            //var categories = _context.Categories.AsNoTracking().AsQueryable();
            var categories = await _categoryRepository.GetAsync(tracking:false);
            if (name is not null)
               categories = categories.Where(e => e.Name.Contains(name)).ToList();

            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(categories.Count() / (double)pageSize);
            categories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList(); 

         
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
        public async Task<IActionResult> Create(Category category)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }
            //_context.Categories.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("Notification", "Category created successfully" , new()
            //{ 

            //});
            TempData["success-notification"] = "Category created successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
           // var category = _context.Categories.Find(id);
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }
            //_context.Categories.Update(category);
            //_context.SaveChanges();
             _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();
            TempData["success-notification"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //var category = _context.Categories.Find(id);
                var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return NotFound();
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
                _categoryRepository.Delete(category);
               await _categoryRepository.CommitAsync();
            TempData["success-notification"] = "Category Deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
