using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Shared.DTOs;
using PointOfSale.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PointOfSale.MVCUI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            var pagedProducts = await _productService.GetPaginatedProductsAsync(pageNumber, pageSize);


            var data = pagedProducts.Items;

            var lst = data.Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.ProductCode
            }).ToList();
            ViewBag.Products = lst;

            return View(pagedProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Delete(string code)
        {
            var product = await _productService.FindProductAsync(code);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string code)
        {
            await _productService.DeleteProductAsync(code);
            return RedirectToAction(nameof(Index));
        }
    }
}
