using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CinemaTicketBooking.Utilities.DbInitialization
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager , ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task Initialize()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SUPER_ADMIN_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.CUSTOMER_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.EMPLOYEE_ROLE));


                await _userManager.CreateAsync(new ApplicationUser
                {
                    FName = "Super",
                    LName = "Admin",
                    Email = "Superadmin@movie.com",
                    EmailConfirmed = true,
                    UserName = "SuperAdmin",
                }, "SuperAdmin123*");
                await _userManager.CreateAsync(new ApplicationUser
                {
                    FName = "Admin",
                    LName = "",
                    Email = "admin@movie.com",
                    EmailConfirmed = true,
                    UserName = "Admin",
                }, "Admin123*");
                await _userManager.CreateAsync(new ApplicationUser
                {
                    FName = "Employee",
                    LName = "1",
                    Email = "employee1@movie.com",
                    EmailConfirmed = true,
                    UserName = "Employee1",
                }, "Employee123*");
                await _userManager.CreateAsync(new ApplicationUser
                {
                    FName = "Employee",
                    LName = "2",
                    Email = "employee2@movie.com",
                    EmailConfirmed = true,
                    UserName = "Employee2",
                }, "Employee123*");
                var user1 = await _userManager.FindByNameAsync("SuperAdmin");
                var user2 = await _userManager.FindByNameAsync("Admin");
                var user3 = await _userManager.FindByNameAsync("Employee1");
                var user4 = await _userManager.FindByNameAsync("Employee2");
                if (user1 != null && user2 != null && user3 != null && user4 != null)
                {
                    await _userManager.AddToRoleAsync(user1, SD.SUPER_ADMIN_ROLE);
                    await _userManager.AddToRoleAsync(user2, SD.ADMIN_ROLE);
                    await _userManager.AddToRoleAsync(user3, SD.EMPLOYEE_ROLE);
                    await _userManager.AddToRoleAsync(user4, SD.EMPLOYEE_ROLE);
                }
            }
        }
    }
}
