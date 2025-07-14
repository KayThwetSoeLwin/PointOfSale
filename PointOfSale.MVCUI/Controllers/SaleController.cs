using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PointOfSale.Interfaces;
using PointOfSale.MVCUI.Filters;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;
using System.Threading.Tasks;

namespace PointOfSale.MVCUI.Controllers;

[Authorize]
public class SaleController : Controller
{
    private readonly ISaleService _saleService;
    private readonly PaginationConfig _paginationConfig;

    public SaleController(ISaleService saleService, IOptions<PaginationConfig> paginationOptions)
    {
        _saleService = saleService;
        _paginationConfig = paginationOptions.Value;
    }

    [PermissionAuthorize("/Sale/Index")]
    public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
    {
        int currentPage = pageNumber ?? _paginationConfig.DefaultPageNumber;
        int currentSize = pageSize ?? _paginationConfig.DefaultPageSize;

        var pagedSales = await _saleService.GetPaginatedSalesAsync(currentPage, currentSize);
        return View(pagedSales);
    }

    [PermissionAuthorize("/Sale/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Sale/Create")]
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

    [PermissionAuthorize("/Sale/Details")]
    public async Task<IActionResult> Details(int id)
    {
        var sale = await _saleService.FindSaleAsync(id);
        if (sale == null)
            return NotFound();

        var details = await _saleService.GetSaleDetailsByVoucherCodeAsync(sale.VoucherCode);
        ViewBag.Details = details;

        return View(sale);
    }

    [PermissionAuthorize("/Sale/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _saleService.FindSaleAsync(id);
        if (sale == null)
            return NotFound();

        return View(sale);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Sale/Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _saleService.DeleteSaleAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
