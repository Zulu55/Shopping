using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;
using Shooping.Enums;
using Shooping.Helpers;

namespace Shooping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckProductsAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.Admin);
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                _context.Products.Add(new Product
                {
                    Description = "AirPods",
                    Name = "AirPods",
                    Price = 1300000M,
                    Stock = 12F,
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Tecnología") }
                    }
                });
                _context.Products.Add(new Product
                {
                    Description = "iPad",
                    Name = "iPad",
                    Price = 2500000M,
                    Stock = 6F,
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Tecnología") }
                    }
                });
                _context.Products.Add(new Product
                {
                    Description = "Mascarilla para la Cara",
                    Name = "Mascarilla para la Cara",
                    Price = 8000M,
                    Stock = 200F,
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Belleza") }
                    }
                });
                _context.Products.Add(new Product
                {
                    Description = "Camisa a Cuadros",
                    Name = "Camisa a Cuadros",
                    Price = 52000M,
                    Stock = 24F,
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Ropa") }
                    }
                });
                _context.Products.Add(new Product
                {
                    Description = "iPhone 13",
                    Name = "iPhone 13",
                    Price = 5200000M,
                    Stock = 8F,
                    ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Tecnología") }
                    }
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Tecnología" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Nutrición" });
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = new List<City>() {
                                new City() { Name = "Medellín" },
                                new City() { Name = "Itagüí" },
                                new City() { Name = "Envigado" },
                                new City() { Name = "Bello" },
                                new City() { Name = "Rionegro" },
                            }
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = new List<City>() {
                                new City() { Name = "Usaquen" },
                                new City() { Name = "Champinero" },
                                new City() { Name = "Santa fe" },
                                new City() { Name = "Useme" },
                                new City() { Name = "Bosa" },
                            }
                        },
                    }
                    });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Florida",
                            Cities = new List<City>() {
                                new City() { Name = "Orlando" },
                                new City() { Name = "Miami" },
                                new City() { Name = "Tampa" },
                                new City() { Name = "Fort Lauderdale" },
                                new City() { Name = "Key West" },
                            }
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = new List<City>() {
                                new City() { Name = "Houston" },
                                new City() { Name = "San Antonio" },
                                new City() { Name = "Dallas" },
                                new City() { Name = "Austin" },
                                new City() { Name = "El Paso" },
                            }
                        },
                    }
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}