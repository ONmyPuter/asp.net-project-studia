using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace CarReservationSystemApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // DbContext + SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity with Roles
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Seed data - roles, admin user, and sample cars
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                
                context.Database.EnsureCreated();
                
                // Create roles if they don't exist
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExistsAsync(roleName).Result)
                    {
                        roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                    }
                }
                
                // Create default admin user
                var adminEmail = "admin@test.com";
                var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = userManager.CreateAsync(adminUser, "Admin123!").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                    }
                }
                
                // Add sample cars if database is empty
                if (!context.Cars.Any())
                {
                    context.Cars.AddRange(
                        new Car { Brand = "Toyota", Model = "Corolla", Seats = 5, FuelType = "Benzyna / Hybryda", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 1 }, // Warszawa
                        new Car { Brand = "Honda", Model = "Civic", Seats = 5, FuelType = "Benzyna / Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 2 }, // Kraków
                        new Car { Brand = "Volkswagen", Model = "Golf", Seats = 5, FuelType = "Benzyna / Diesel / e-Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 3 }, // Gdańsk
                        new Car { Brand = "Skoda", Model = "Octavia", Seats = 5, FuelType = "Benzyna / Diesel / e-Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 4 }, // Wrocław
                        new Car { Brand = "Ford", Model = "Mustang", Seats = 4, FuelType = "Benzyna", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 5 }, // Poznań
                        new Car { Brand = "Aston Martin", Model = "DB11", Seats = 4, FuelType = "Benzyna", Transmission = "Automatyczna", IsAvailable = true, CurrentLocationId = 1 }, // Warszawa
                        new Car { Brand = "Audi", Model = "Q5", Seats = 5, FuelType = "Benzyna / Diesel / Plug-in Hybrid", Transmission = "Automatyczna", IsAvailable = true, CurrentLocationId = 2 }, // Kraków
                        new Car { Brand = "Tesla", Model = "Model 3", Seats = 5, FuelType = "Elektryczny", Transmission = "Automatyczna (jedbiegowa)", IsAvailable = true, CurrentLocationId = 3 }, // Gdańsk
                        new Car { Brand = "Saab", Model = "9-3", Seats = 5, FuelType = "Benzyna / Diesel*", Transmission = "Manualna / Automatyczna*", IsAvailable = true, CurrentLocationId = 4 }, // Wrocław
                        new Car { Brand = "Mercedes-Benz", Model = "V-Class", Seats = 7, FuelType = "Diesel", Transmission = "Automatyczna", IsAvailable = true, CurrentLocationId = 5 }, // Poznań
                        new Car { Brand = "BMW", Model = "E36", Seats = 5, FuelType = "Benzyna / Diesel", Transmission = "Manualna / Automatyczna", IsAvailable = true, CurrentLocationId = 1 } // Warszawa
                    );
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
