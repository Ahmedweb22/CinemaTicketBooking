using System.Globalization;
using CinemaTicketBooking.Utilities.DbInitialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Stripe;

namespace CinemaTicketBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(
               option =>
               { 
               option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") );
               }
                );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>( options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IRepository<Actors>, Repository<Actors>>();
            builder.Services.AddScoped<IMovieSubImgRepository, MovieSubImgRepository>();
            builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            builder.Services.AddScoped <IRepository<Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            builder.Services.AddScoped<IAccountService, AccountService>();
     

            builder.Services.AddTransient<IEmailSender , EmailSender>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            const string defaultCulture = "en";
            var supportedCultures = new[] 
            {
            new CultureInfo(defaultCulture),
            new CultureInfo("ar"),
            new CultureInfo("es")
             };
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        { 
           options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseRequestLocalization
                (app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            service.Initialize();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
