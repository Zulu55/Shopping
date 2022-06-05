using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Enums;
using Shooping.Helpers;
using Vereyon.Web;
using static Shooping.Helpers.ModalHelper;

namespace Shooping.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        private readonly IOrdersHelper _ordersHelper;

        public OrdersController(DataContext context, IFlashMessage flashMessage, IOrdersHelper ordersHelper)
        {
            _context = context;
            _flashMessage = flashMessage;
            _ordersHelper = ordersHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        [NoDirectAccess]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dispatch(int id)
        {
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispatch(Sale sale)
        {
            if (sale.OrderStatus != OrderStatus.Nuevo)
            {
                _flashMessage.Danger("Solo se pueden despachar pedidos que estén en estado 'nuevo'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
            else
            {
                sale.OrderStatus = OrderStatus.Despachado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("El estado del pedido ha sido cambiado a 'despachado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }

        }

        [NoDirectAccess]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send(int? id)
        {
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(Sale sale)
        {
            if (sale.OrderStatus != OrderStatus.Despachado)
            {
                _flashMessage.Danger("Solo se pueden despachar pedidos que estén en estado 'Despachado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
            else
            {
                sale.OrderStatus = OrderStatus.Enviado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("El estado del pedido ha sido cambiado a 'Enviado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
        }

        [NoDirectAccess]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int? id)
        {
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(Sale sale)
        {
            if (sale.OrderStatus != OrderStatus.Enviado)
            {
                _flashMessage.Danger("Solo se pueden despachar pedidos que estén en estado 'Enviado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
            else
            {
                sale.OrderStatus = OrderStatus.Confirmado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("El estado del pedido ha sido cambiado a 'Confirmado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
        }
        [NoDirectAccess]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int? id)
        {
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Sale sale)
        {

            if (sale.OrderStatus == OrderStatus.Cancelado)
            {
                _flashMessage.Danger("No se puede cancelar un pedido que esté en estado 'cancelado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }
            else
            {
                await _ordersHelper.CancelOrderAsync(sale.Id);
                _flashMessage.Confirmation("El estado del pedido ha sido cambiado a 'cancelado'.");
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", sale) });
            }

        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyOrders()
        {
            return View(await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .Where(s => s.User.UserName == User.Identity.Name)
                .ToListAsync());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
    }
}