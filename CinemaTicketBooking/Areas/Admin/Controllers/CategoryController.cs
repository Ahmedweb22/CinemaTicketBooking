using CinemaTicketBooking.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class CategoryController : Controller
    {
        //private ApplicationDbContext _context = new();
        //private Repository<Category> _categoryRepository = new();
        private IRepository<Category> _categoryRepository;
        private readonly IStringLocalizer<LocalizationController> _localizer;
        public CategoryController(IRepository<Category> categoryRepository, IStringLocalizer<LocalizationController> localizer)
        {
            _categoryRepository = categoryRepository;
            _localizer = localizer;
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
            TempData["success-notification"] = _localizer["CreateCategory"].Value;

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
           // var category = _context.Categories.Find(id);
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
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
            TempData["success-notification"] = _localizer["UpdateCategory"].Value;
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
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
            TempData["success-notification"] = _localizer["DeleteCategory"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
