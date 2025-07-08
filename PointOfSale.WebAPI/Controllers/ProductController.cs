using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;

namespace PointOfSale.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result); // result is already List<ProductDto>
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var product = await _productService.FindProductAsync(code);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product); // product is ProductDto
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequestModel request)
        {
            var result = await _productService.CreateProductAsync(request);

            return result.IsSuccess
                ? Ok(new { message = result.Message })
                : BadRequest(new { message = result.Message });
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, [FromBody] ProductUpdateRequestModel request)
        {
            request.ProductCode = code;

            var result = await _productService.UpdateProductAsync(request);

            return result.IsSuccess
                ? Ok(new { message = result.Message })
                : NotFound(new { message = result.Message });
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var result = await _productService.DeleteProductAsync(code);

            return result > 0
                ? Ok(new { message = "Product deleted successfully" })
                : NotFound(new { message = "Product not found" });
        }
    }
}
