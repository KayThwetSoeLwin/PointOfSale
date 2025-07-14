using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using PointOfSale.Interfaces;
using PointOfSale.MVCUI.Filters;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.MVCUI.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly PaginationConfig _paginationConfig;

    public ProductController(IProductService productService, IOptions<PaginationConfig> paginationOptions)
    {
        _productService = productService;
        _paginationConfig = paginationOptions.Value;
    }

    [PermissionAuthorize("/Product/Index")]
    public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
    {
        int currentPage = pageNumber ?? _paginationConfig.DefaultPageNumber;
        int currentSize = pageSize ?? _paginationConfig.DefaultPageSize;

        var pagedProducts = await _productService.GetPaginatedProductsAsync(currentPage, currentSize);

        var data = pagedProducts.Items;

        var lst = data.Select(x => new SelectListItem
        {
            Text = x.ProductName,
            Value = x.ProductCode
        }).ToList();

        ViewBag.Products = lst;

        return View(pagedProducts);
    }

    [PermissionAuthorize("/Product/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Product/Create")]
    public async Task<IActionResult> Create(ProductCreateRequestModel product)
    {
        if (!ModelState.IsValid)
            return View(product);

        var result = await _productService.CreateProductAsync(product);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", result.Message);
        return View(product);
    }

    [PermissionAuthorize("/Product/Edit")]
    public async Task<IActionResult> Edit(string code)
    {
        var product = await _productService.FindProductAsync(code);
        if (product == null)
            return NotFound();

        var request = new ProductUpdateRequestModel
        {
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Product/Edit")]
    public async Task<IActionResult> Edit(string code, ProductUpdateRequestModel request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _productService.UpdateProductAsync(request);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", result.Message);
        return View(request);
    }

    [PermissionAuthorize("/Product/Delete")]
    public async Task<IActionResult> Delete(string code)
    {
        var product = await _productService.FindProductAsync(code);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Product/Delete")]
    public async Task<IActionResult> DeleteConfirmed(string code)
    {
        await _productService.DeleteProductAsync(code);
        return RedirectToAction(nameof(Index));
    }
}
