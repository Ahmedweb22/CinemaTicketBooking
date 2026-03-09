using System.Diagnostics;
using CinemaTicketBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketBooking.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IRepository<Movie> _movieRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Cinema> _cinemaRepository;
        private IRepository<Actors> _actorRepository;


        public HomeController(ILogger<HomeController> logger, IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actors> actorRepository )
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(int? categoryId , int? cinemaId)
        {
       
             var movies = await _movieRepository.GetAsync(m => m.Price >= 300,
                 includes: [m => m.Category, m => m.Cinema, m => m.Actors]);

           // movies = movies.Where(m => m.Price <= 300).ToList();
           if (categoryId != null)
            {
                movies = movies.Where(m => m.CategoryId == categoryId).ToList();
            }
            if (cinemaId != null)
            {
                movies.Where(m => m.CinemaId == cinemaId).ToList();
            }
            movies = movies.Skip(0)
                .Take(8)
                .ToList();
            var categories = await _categoryRepository.GetAsync();
             var cinemas = await _cinemaRepository.GetAsync();
            var actors = await _actorRepository.GetAsync();
                actors = actors.Skip(0)
                .Take(12)
                .ToList();
        
             return View(new HomeVM
             {
                 Movies = movies.ToList(),
                 Categories = categories.ToList(),
                 Cinemas = cinemas.ToList(),
                 Actors = actors.ToList()
             });
           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
