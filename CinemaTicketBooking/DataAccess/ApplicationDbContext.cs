using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaTicketBooking.ViewModels;

namespace CinemaTicketBooking.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }
        public DbSet<MovieSubImgs> MovieSubImgs { get; set; }
        public DbSet<Movie> Movies { get; set; }        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actors> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
     public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
public DbSet<Cart> Carts { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-9Q6DBR4\\SQL22; Initial Catalog=CinemaTicketBooking;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        //}
    }
}
