#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Vereyon.Web;

namespace Shooping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CategoriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories
                .Include(c => c.ProductCategories)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            Category category = new() {  ProductCategories = new List<ProductCategory>() };
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe una categoría con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe una categoría con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(category);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(category);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
