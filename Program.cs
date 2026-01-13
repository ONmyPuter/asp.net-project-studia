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

            // Identity
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
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

            // Seed danych - dodaj przykładowe samochody jeśli baza jest pusta
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                
                // Sprawdź i dodaj kolumny FuelType i Transmission, jeśli nie istnieją
                var connection = context.Database.GetDbConnection();
                connection.Open();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        // Sprawdź czy kolumna FuelType istnieje
                        command.CommandText = "PRAGMA table_info(Cars)";
                        using (var reader = command.ExecuteReader())
                        {
                            bool hasFuelType = false;
                            bool hasTransmission = false;
                            
                            while (reader.Read())
                            {
                                var columnName = reader.GetString(1);
                                if (columnName == "FuelType") hasFuelType = true;
                                if (columnName == "Transmission") hasTransmission = true;
                            }
                            
                            // Dodaj kolumnę FuelType jeśli nie istnieje
                            if (!hasFuelType)
                            {
                                using (var addColumnCommand = connection.CreateCommand())
                                {
                                    addColumnCommand.CommandText = "ALTER TABLE Cars ADD COLUMN FuelType TEXT DEFAULT ''";
                                    addColumnCommand.ExecuteNonQuery();
                                }
                            }
                            
                            // Dodaj kolumnę Transmission jeśli nie istnieje
                            if (!hasTransmission)
                            {
                                using (var addColumnCommand = connection.CreateCommand())
                                {
                                    addColumnCommand.CommandText = "ALTER TABLE Cars ADD COLUMN Transmission TEXT DEFAULT ''";
                                    addColumnCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
                
                if (!context.Cars.Any())
                {
                    context.Cars.AddRange(
                        new Car { Brand = "Toyota", Model = "Corolla", Seats = 5, FuelType = "Benzyna / Hybryda", Transmission = "Manualna / Automatyczna", IsAvailable = true },
                        new Car { Brand = "Honda", Model = "Civic", Seats = 5, FuelType = "Benzyna / Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true },
                        new Car { Brand = "Volkswagen", Model = "Golf", Seats = 5, FuelType = "Benzyna / Diesel / e-Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true },
                        new Car { Brand = "Skoda", Model = "Octavia", Seats = 5, FuelType = "Benzyna / Diesel / e-Hybrid", Transmission = "Manualna / Automatyczna", IsAvailable = true },
                        new Car { Brand = "Ford", Model = "Mustang", Seats = 4, FuelType = "Benzyna", Transmission = "Manualna / Automatyczna", IsAvailable = true },
                        new Car { Brand = "Aston Martin", Model = "DB11", Seats = 4, FuelType = "Benzyna", Transmission = "Automatyczna", IsAvailable = true },
                        new Car { Brand = "Audi", Model = "Q5", Seats = 5, FuelType = "Benzyna / Diesel / Plug-in Hybrid", Transmission = "Automatyczna", IsAvailable = true },
                        new Car { Brand = "Tesla", Model = "Model 3", Seats = 5, FuelType = "Elektryczny", Transmission = "Automatyczna (jedbiegowa)", IsAvailable = true },
                        new Car { Brand = "Saab", Model = "9-3", Seats = 5, FuelType = "Benzyna / Diesel*", Transmission = "Manualna / Automatyczna*", IsAvailable = true },
                        new Car { Brand = "Mercedes-Benz", Model = "V-Class", Seats = 7, FuelType = "Diesel", Transmission = "Automatyczna", IsAvailable = true },
                        new Car { Brand = "BMW", Model = "E36", Seats = 5, FuelType = "Benzyna / Diesel", Transmission = "Manualna / Automatyczna", IsAvailable = true }
                    );
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
