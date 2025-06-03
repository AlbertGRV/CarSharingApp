using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarSharingApp.Models;

namespace CarSharingApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@carsharing.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Администратор",
                    LastName = "Системы",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create test user
            var userEmail = "user@test.com";
            var testUser = await userManager.FindByEmailAsync(userEmail);
            if (testUser == null)
            {
                testUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "Иван",
                    LastName = "Петров",
                    EmailConfirmed = true,
                    DateOfBirth = new DateTime(1990, 5, 15),
                    DriverLicenseNumber = "1234567890"
                };
                await userManager.CreateAsync(testUser, "User123!");
                await userManager.AddToRoleAsync(testUser, "User");
            }

            // Seed cars if none exist
            if (!context.Cars.Any())
            {
                var cars = new List<Car>
                {
                    // Существующие автомобили
                    new Car
                    {
                        Brand = "Toyota",
                        Model = "Camry",
                        Year = 2023,
                        Color = "Белый",
                        LicensePlate = "А123БВ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 8.50m,
                        FuelLevel = 85,
                        Mileage = 15000,
                        Latitude = 55.7558,
                        Longitude = 37.6176,
                        Address = "ул. Тверская, 15",
                        Status = CarStatus.Available,
                        Description = "Комфортный седан для городских поездок"
                    },
                    new Car
                    {
                        Brand = "BMW",
                        Model = "X3",
                        Year = 2022,
                        Color = "Черный",
                        LicensePlate = "В456ГД777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 12.00m,
                        FuelLevel = 92,
                        Mileage = 8500,
                        Latitude = 55.7522,
                        Longitude = 37.6156,
                        Address = "Красная площадь, 1",
                        Status = CarStatus.Available,
                        Description = "Премиальный кроссовер BMW"
                    },
                    new Car
                    {
                        Brand = "Mercedes-Benz",
                        Model = "C-Class",
                        Year = 2023,
                        Color = "Серебристый",
                        LicensePlate = "Е789ЖЗ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 15.00m,
                        FuelLevel = 78,
                        Mileage = 5200,
                        Latitude = 55.7539,
                        Longitude = 37.6208,
                        Address = "ул. Арбат, 25",
                        Status = CarStatus.Available,
                        Description = "Элегантный Mercedes-Benz C-Class"
                    },
                    new Car
                    {
                        Brand = "Audi",
                        Model = "A4",
                        Year = 2022,
                        Color = "Синий",
                        LicensePlate = "И012КЛ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 13.50m,
                        FuelLevel = 95,
                        Mileage = 12000,
                        Latitude = 55.7601,
                        Longitude = 37.6184,
                        Address = "Театральная площадь, 1",
                        Status = CarStatus.Available,
                        Description = "Спортивный седан Audi A4"
                    },
                    new Car
                    {
                        Brand = "Tesla",
                        Model = "Model 3",
                        Year = 2023,
                        Color = "Красный",
                        LicensePlate = "М345НО777",
                        FuelType = FuelType.Electric,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 10.00m,
                        FuelLevel = 88,
                        Mileage = 3500,
                        Latitude = 55.7485,
                        Longitude = 37.6232,
                        Address = "Парк Горького",
                        Status = CarStatus.Available,
                        Description = "Электромобиль Tesla Model 3"
                    },

                    // НОВЫЕ АВТОМОБИЛИ
                    
                    // Эконом класс
                    new Car
                    {
                        Brand = "Lada",
                        Model = "Granta",
                        Year = 2022,
                        Color = "Белый",
                        LicensePlate = "П678РС777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Manual,
                        SeatsCount = 5,
                        PricePerMinute = 4.50m,
                        FuelLevel = 70,
                        Mileage = 25000,
                        Latitude = 55.7312,
                        Longitude = 37.6016,
                        Address = "ул. Ленина, 45",
                        Status = CarStatus.Available,
                        Description = "Экономичный автомобиль для ежедневных поездок"
                    },
                    new Car
                    {
                        Brand = "Renault",
                        Model = "Logan",
                        Year = 2021,
                        Color = "Серый",
                        LicensePlate = "Т901УФ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Manual,
                        SeatsCount = 5,
                        PricePerMinute = 5.00m,
                        FuelLevel = 65,
                        Mileage = 32000,
                        Latitude = 55.7887,
                        Longitude = 37.6693,
                        Address = "Сокольники, парк",
                        Status = CarStatus.Available,
                        Description = "Надежный седан Renault Logan"
                    },
                    new Car
                    {
                        Brand = "Hyundai",
                        Model = "Solaris",
                        Year = 2023,
                        Color = "Красный",
                        LicensePlate = "Х234ЦЧ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 6.50m,
                        FuelLevel = 80,
                        Mileage = 8000,
                        Latitude = 55.7644,
                        Longitude = 37.5934,
                        Address = "Патриаршие пруды",
                        Status = CarStatus.Available,
                        Description = "Современный Hyundai Solaris"
                    },
                    new Car
                    {
                        Brand = "Kia",
                        Model = "Rio",
                        Year = 2022,
                        Color = "Синий",
                        LicensePlate = "Ш567ЩЭ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 6.00m,
                        FuelLevel = 75,
                        Mileage = 18000,
                        Latitude = 55.7423,
                        Longitude = 37.5906,
                        Address = "ул. Остоженка, 12",
                        Status = CarStatus.Available,
                        Description = "Стильный хэтчбек Kia Rio"
                    },

                    // Комфорт класс
                    new Car
                    {
                        Brand = "Volkswagen",
                        Model = "Polo",
                        Year = 2023,
                        Color = "Белый",
                        LicensePlate = "Ю890ЯА777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 7.50m,
                        FuelLevel = 90,
                        Mileage = 5000,
                        Latitude = 55.7520,
                        Longitude = 37.5844,
                        Address = "Новый Арбат, 15",
                        Status = CarStatus.Available,
                        Description = "Качественный Volkswagen Polo"
                    },
                    new Car
                    {
                        Brand = "Skoda",
                        Model = "Rapid",
                        Year = 2022,
                        Color = "Серебристый",
                        LicensePlate = "Б123ВГ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 7.00m,
                        FuelLevel = 85,
                        Mileage = 15000,
                        Latitude = 55.7156,
                        Longitude = 37.5515,
                        Address = "Лужники, стадион",
                        Status = CarStatus.Available,
                        Description = "Просторный Skoda Rapid"
                    },
                    new Car
                    {
                        Brand = "Nissan",
                        Model = "Almera",
                        Year = 2023,
                        Color = "Черный",
                        LicensePlate = "Д456ЕЖ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 7.80m,
                        FuelLevel = 88,
                        Mileage = 6500,
                        Latitude = 55.7964,
                        Longitude = 37.7516,
                        Address = "Измайлово, парк",
                        Status = CarStatus.Available,
                        Description = "Комфортный Nissan Almera"
                    },

                    // Внедорожники
                    new Car
                    {
                        Brand = "Toyota",
                        Model = "RAV4",
                        Year = 2023,
                        Color = "Серый",
                        LicensePlate = "З789ИК777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 11.50m,
                        FuelLevel = 92,
                        Mileage = 7200,
                        Latitude = 55.8304,
                        Longitude = 37.6327,
                        Address = "ВДНХ, главный вход",
                        Status = CarStatus.Available,
                        Description = "Надежный кроссовер Toyota RAV4"
                    },
                    new Car
                    {
                        Brand = "Mazda",
                        Model = "CX-5",
                        Year = 2022,
                        Color = "Красный",
                        LicensePlate = "Л012МН777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 10.50m,
                        FuelLevel = 78,
                        Mileage = 12500,
                        Latitude = 55.7570,
                        Longitude = 37.4147,
                        Address = "Крылатское, набережная",
                        Status = CarStatus.Available,
                        Description = "Спортивный кроссовер Mazda CX-5"
                    },
                    new Car
                    {
                        Brand = "Mitsubishi",
                        Model = "Outlander",
                        Year = 2023,
                        Color = "Белый",
                        LicensePlate = "О345ПР777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 7,
                        PricePerMinute = 12.50m,
                        FuelLevel = 85,
                        Mileage = 4800,
                        Latitude = 55.6037,
                        Longitude = 37.7438,
                        Address = "Братеево, парк",
                        Status = CarStatus.Available,
                        Description = "Семейный внедорожник Mitsubishi Outlander"
                    },

                    // Премиум класс
                    new Car
                    {
                        Brand = "BMW",
                        Model = "5 Series",
                        Year = 2023,
                        Color = "Черный",
                        LicensePlate = "С678ТУ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 18.00m,
                        FuelLevel = 90,
                        Mileage = 3200,
                        Latitude = 55.7558,
                        Longitude = 37.4181,
                        Address = "Рублевка, ТЦ",
                        Status = CarStatus.Available,
                        Description = "Представительский BMW 5 Series"
                    },
                    new Car
                    {
                        Brand = "Mercedes-Benz",
                        Model = "E-Class",
                        Year = 2023,
                        Color = "Серебристый",
                        LicensePlate = "Ф901ХЦ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 20.00m,
                        FuelLevel = 95,
                        Mileage = 2100,
                        Latitude = 55.4108,
                        Longitude = 37.9034,
                        Address = "Домодедово, аэропорт",
                        Status = CarStatus.Available,
                        Description = "Роскошный Mercedes-Benz E-Class"
                    },
                    new Car
                    {
                        Brand = "Lexus",
                        Model = "ES",
                        Year = 2023,
                        Color = "Белый перламутр",
                        LicensePlate = "Ч234ШЩ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 17.50m,
                        FuelLevel = 88,
                        Mileage = 4500,
                        Latitude = 55.6761,
                        Longitude = 37.5725,
                        Address = "Нагорная, метро",
                        Status = CarStatus.Available,
                        Description = "Премиальный седан Lexus ES"
                    },

                    // Электромобили
                    new Car
                    {
                        Brand = "Nissan",
                        Model = "Leaf",
                        Year = 2022,
                        Color = "Синий",
                        LicensePlate = "Ъ567ЫЬ777",
                        FuelType = FuelType.Electric,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 9.00m,
                        FuelLevel = 75,
                        Mileage = 18000,
                        Latitude = 55.7312,
                        Longitude = 37.6016,
                        Address = "Парк Сокольники",
                        Status = CarStatus.Available,
                        Description = "Экологичный Nissan Leaf"
                    },
                    new Car
                    {
                        Brand = "BMW",
                        Model = "i3",
                        Year = 2022,
                        Color = "Белый",
                        LicensePlate = "Э890ЮЯ777",
                        FuelType = FuelType.Electric,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 4,
                        PricePerMinute = 14.00m,
                        FuelLevel = 82,
                        Mileage = 12000,
                        Latitude = 55.7887,
                        Longitude = 37.6693,
                        Address = "Сокольники, аллея",
                        Status = CarStatus.Available,
                        Description = "Инновационный BMW i3"
                    },

                    // Автомобили с разными статусами
                    new Car
                    {
                        Brand = "Ford",
                        Model = "Focus",
                        Year = 2021,
                        Color = "Серый",
                        LicensePlate = "А111БВ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 6.80m,
                        FuelLevel = 45,
                        Mileage = 35000,
                        Latitude = 55.7644,
                        Longitude = 37.5934,
                        Address = "Сервисный центр",
                        Status = CarStatus.Maintenance,
                        Description = "Ford Focus на техническом обслуживании"
                    },
                    new Car
                    {
                        Brand = "Chevrolet",
                        Model = "Cruze",
                        Year = 2020,
                        Color = "Черный",
                        LicensePlate = "Г222ДЕ777",
                        FuelType = FuelType.Gasoline,
                        TransmissionType = TransmissionType.Automatic,
                        SeatsCount = 5,
                        PricePerMinute = 6.50m,
                        FuelLevel = 30,
                        Mileage = 45000,
                        Latitude = 55.7423,
                        Longitude = 37.5906,
                        Address = "В аренде",
                        Status = CarStatus.Booked,
                        Description = "Chevrolet Cruze в аренде"
                    }
                };

                context.Cars.AddRange(cars);
                await context.SaveChangesAsync();
            }
        }
    }
}