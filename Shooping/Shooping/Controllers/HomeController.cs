using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Common;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Helpers;
using Shooping.Models;
using System.Diagnostics;

namespace Shooping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IOrdersHelper _ordersHelper;
        private readonly ICombosHelper _combosHelper;

        public HomeController(ILogger<HomeController> logger, DataContext context, IUserHelper userHelper, IOrdersHelper ordersHelper, ICombosHelper combosHelper)
        {
            _logger = logger;
            _context = context;
            _userHelper = userHelper;
            _ordersHelper = ordersHelper;
            _combosHelper = combosHelper;
        }

            [HttpGet]
            [HttpPost]
            public async Task<IActionResult> Index(HomeViewModel? model)
            {
            List<Product>? products = await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                    .Where(p => p.Stock > 0)
                    .OrderBy(p => p.Description)
                    .ToListAsync();

            if (model.CategoryId != 0)
            {
                products = products
                    .Where(p => p.ProductCategories.Any(pc => pc.Category.Id == model.CategoryId))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(model.FilterName))
            {
                products = products
                    .Where(p => p.Name.ToLower().Contains(model.FilterName.ToLower()))
                    .ToList();
            }

            List<ProductsHomeViewModel> productsHome = new() { new ProductsHomeViewModel() };
            int i = 1;
            foreach (Product? product in products)
            {
                if (i == 1)
                {
                    productsHome.LastOrDefault().Product1 = product;
                }
                if (i == 2)
                {
                    productsHome.LastOrDefault().Product2 = product;
                }
                if (i == 3)
                {
                    productsHome.LastOrDefault().Product3 = product;
                }
                if (i == 4)
                {
                    productsHome.LastOrDefault().Product4 = product;
                    productsHome.Add(new ProductsHomeViewModel());
                    i = 0;
                }
                i++;
            }

            model.Products = productsHome;
            model.Categories = await _combosHelper.GetComboCategoriesAsync(true);

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user != null)
            {
                model.Quantity = await _context.TemporalSales
                    .Where(ts => ts.User.Id == user.Id)
                    .SumAsync(ts => ts.Quantity);
            }

            return View(model);
        }


        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = new()
            {
                Product = product,
                Quantity = 1,
                User = user
            };

            _context.TemporalSales.Add(temporalSale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            string categories = string.Empty;
            foreach (ProductCategory? category in product.ProductCategories)
            {
                categories += $"{category.Category.Name}, ";
            }
            categories = categories.Substring(0, categories.Length - 2);

            AddProductToCartViewModel model = new()
            {
                Categories = categories,
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ProductImages = product.ProductImages,
                Quantity = 1,
                Stock = product.Stock,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(AddProductToCartViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            Product product = await _context.Products.FindAsync(model.Id);
            if (product == null)
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = new()
            {
                Product = product,
                Quantity = model.Quantity,
                Remarks = model.Remarks,
                User = user
            };

            _context.TemporalSales.Add(temporalSale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> ShowCart()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            List<TemporalSale>? temporalSales = await _context.TemporalSales
                .Include(ts => ts.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(ts => ts.User.Id == user.Id)
                .ToListAsync();

            ShowCartViewModel model = new()
            {
                User = user,
                TemporalSales = temporalSales,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowCart(ShowCartViewModel model)
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            model.User = user;
            model.TemporalSales = await _context.TemporalSales
                .Include(ts => ts.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(ts => ts.User.Id == user.Id)
                .ToListAsync();

            Response response = await _ordersHelper.ProcessOrderAsync(model);
            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(OrderSuccess));
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(model);
        }

        [Authorize]
        public IActionResult OrderSuccess()
        {
            return View();
        }

        public async Task<IActionResult> DecreaseQuantity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            if (temporalSale.Quantity > 1)
            {
                temporalSale.Quantity--;
                _context.TemporalSales.Update(temporalSale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> IncreaseQuantity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            temporalSale.Quantity++;
            _context.TemporalSales.Update(temporalSale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            _context.TemporalSales.Remove(temporalSale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            EditTemporalSale model = new()
            {
                Id = temporalSale.Id,
                Quantity = temporalSale.Quantity,
                Remarks = temporalSale.Remarks,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTemporalSale model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TemporalSale temporalSale = await _context.TemporalSales.FindAsync(id);
                    temporalSale.Quantity = model.Quantity;
                    temporalSale.Remarks = model.Remarks;
                    _context.Update(temporalSale);
                    await _context.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                    return View(model);
                }

                return RedirectToAction(nameof(ShowCart));
            }

            return View(model);
        }
    }
}