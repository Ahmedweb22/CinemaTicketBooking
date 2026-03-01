using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
namespace CinemaTicketBooking.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<Movie> _movieRepo;
        private readonly IRepository<Promotion> _promotionRepo;
        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepo, IRepository<Movie> movieRepo, IRepository<Promotion> promotionRepo)
        {
            _userManager = userManager;
            _cartRepo = cartRepo;
            _movieRepo = movieRepo;
            _promotionRepo = promotionRepo;
        }
        public async Task<IActionResult> AddToCart(int movieId)
        {
            var user= await _userManager.GetUserAsync(User);
            var movie = await _movieRepo.GetOneAsync(m => m.Id == movieId);
            if (user == null || movie == null) return NotFound();
            await _cartRepo.CreateAsync(new Cart
            {
                MovieId = movieId,
                ApplicationUserId = user.Id,
                ListPrice = movie.Price,

            });
            await _cartRepo.CommitAsync();
            TempData["success-notification"] = "Movie added to cart successfully!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Index(string? code = null)
        { 
            var user= await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
             var carts = await _cartRepo.GetAsync(c => c.ApplicationUserId == user.Id , includes:[c =>c.Movie]);
            if (code is not null)
            {
                var promotion = await _promotionRepo.GetAsync(p => p.Code == code && p.IsValid);
                    var movieIds = promotion.Select(p => p.MovieId);
      

                foreach (var cart in carts)
                {
                    if (movieIds.Contains(cart.MovieId))
                    {
                        var discount = promotion.Select(p => p.Discount).FirstOrDefault();
                        cart.ListPrice -= cart.ListPrice * discount / 100;
                        await _promotionRepo.CommitAsync();

                        TempData["success-notification"] = "Promotion applied successfully!";
                    }
                    else
                    {
                        TempData["error-notification"] = "Promotion code is not applicable for the movies in your cart.";
                    }
                }
            }  
            
            return View(carts);
        }
        public async Task<IActionResult> Delete(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var cart = await _cartRepo.GetOneAsync(c => c.ApplicationUserId == user.Id && c.MovieId == movieId);
            if (cart == null) return NotFound();
             _cartRepo.Delete(cart);
            await _cartRepo.CommitAsync();
            TempData["success-notification"] = "Movie removed from cart successfully!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = Url.Action($"{Request.Scheme}://{Request.Host}/Customer/Checkout/success"),
                CancelUrl = Url.Action($"{Request.Scheme}://{Request.Host}/Customer/Checkout/cancel"),
            };
         
            var carts = await _cartRepo.GetAsync(c => c.ApplicationUserId == user.Id, includes: [c => c.Movie]);
            if (carts == null) return NotFound();

            foreach (var cart in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Movie.Name,
                            Description = cart.Movie.Description,
                        },
                        UnitAmount = (long)(cart.ListPrice * 100), // Convert to cents
                    },
                });
            }
           
            
            var service = new SessionService();
            var session = service.Create(options);
            return Ok(new { sessionId = session.Id });
        }
    }
}
