using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;

namespace PointOfSale.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: api/sale
        [HttpGet]
        public async Task<IActionResult> GetAllSalesAsync()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(new { message = "Sales retrieved successfully", data = sales });
        }

        // GET: api/sale/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleByIdAsync(int id)
        {
            var sale = await _saleService.FindSaleAsync(id);
            if (sale == null)
                return NotFound(new { message = "Sale not found." });

            return Ok(new { message = "Sale found", data = sale });
        }

        // POST: api/sale
        [HttpPost]
        public async Task<IActionResult> CreateSaleAsync([FromBody] SaleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data." });

            var result = await _saleService.CreateSaleWithDetailsAsync(request);
            return result > 0
                ? Ok(new { message = "Sale created successfully." })
                : BadRequest(new { message = "Failed to create sale." });
        }

        // DELETE: api/sale/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleAsync(int id)
        {
            var result = await _saleService.DeleteSaleAsync(id);
            return result > 0
                ? Ok(new { message = "Sale deleted (soft delete)." })
                : NotFound(new { message = "Sale not found." });
        }
    }
}
