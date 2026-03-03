using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize]
    public class PromotionController : Controller
    {
        private IRepository<Promotion> _promotionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private IRepository<Movie> _movieRepository;
        public PromotionController(IRepository<Promotion> promotionRepository, UserManager<ApplicationUser> userManager, IRepository<Movie> movieRepository)
        {
            _promotionRepository = promotionRepository;
            _userManager = userManager;
            _movieRepository = movieRepository;
           
        }
        public async Task<IActionResult> Index(string? code, int page = 1)
        {
            var promotions = await _promotionRepository.GetAsync(includes: [m => m.Movie, m => m.ApplicationUser], tracking: false);

                var movies = await _movieRepository.GetAsync(tracking: false);
            var users = _userManager.Users.AsNoTracking().AsQueryable();
            if (code is not null)
                promotions = promotions.Where(e => e.Code.Contains(code)).ToList();

            if (page < 1)
                page = 1;
            int pageSize = 10;
            int currentPage = page;
            double totalCount = Math.Ceiling(promotions.Count() / (double)pageSize);
            promotions = promotions.Skip((page - 1) * pageSize).Take(pageSize).ToList();


            return View(new PromotionsVM
            {
                Promotions = promotions,
                Movies = movies,
                Users = users,
                TotalPages = totalCount,
                CurrentPage = currentPage
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var movies = await _movieRepository.GetAsync(tracking: false);
            var users = _userManager.Users.AsNoTracking().AsQueryable();
            return View(new CreatePromotionVM
            {
                Movies = movies,
                Users = users
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePromotionVM promotionVM)
        {
            ModelState.Remove("Movie");
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("Promotion.Id");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                promotionVM.Movies = await _movieRepository.GetAsync(tracking: false);
                promotionVM.Users = _userManager.Users.AsNoTracking().AsQueryable();
                return View(promotionVM);
            }
            await _promotionRepository.CreateAsync(promotionVM.Promotion);
            await _promotionRepository.CommitAsync();


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
  {    
            var promotion = await _promotionRepository.GetOneAsync(e => e.Id == id);
            var movies = await _movieRepository.GetAsync(tracking: false);
            var users = _userManager.Users.AsNoTracking().AsQueryable();
            if (promotion is null)
                return NotFound();
            return View(new PromotionUpdateResponceVM
            {
                promotion = promotion,
                movies = movies,
                Users = users
            });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(PromotionUpdateResponceVM promotionVM)
        {
            ModelState.Remove("Movie");
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("Promotion.Id");
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                promotionVM.movies = await _movieRepository.GetAsync(tracking: false);
                promotionVM.Users = _userManager.Users.AsNoTracking().AsQueryable();
                return View(promotionVM);
            }
           
            _promotionRepository.Update(promotionVM.promotion);
            await _promotionRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var promotion = await _promotionRepository.GetOneAsync(e => e.Id == id);
            if (promotion is null)
                return NotFound();
            _promotionRepository.Delete(promotion);
            await _promotionRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
