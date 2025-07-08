using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;

namespace PointOfSale.MVCUI.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            var pagedSales = await _saleService.GetPaginatedSalesAsync(pageNumber, pageSize);

            return View(pagedSales);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input.");
                return View(request);
            }

            var result = await _saleService.CreateSaleWithDetailsAsync(request);
            if (result > 0)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create sale.");
            return View(request);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sale = await _saleService.FindSaleAsync(id);
            if (sale == null)
                return NotFound();

            var details = await _saleService.GetSaleDetailsByVoucherCodeAsync(sale.VoucherCode);
            ViewBag.Details = details;

            return View(sale);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _saleService.FindSaleAsync(id);
            if (sale == null)
                return NotFound();

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _saleService.DeleteSaleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
