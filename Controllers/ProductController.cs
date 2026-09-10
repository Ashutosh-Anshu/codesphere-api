using codesphere_api.DTOs;
using codesphere_api.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace codesphere_api.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("createAsync")]
        public async Task<IActionResult> CreateAsync(
            [FromBody] ProductDTO product,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.CreateAsync(product, cancellationToken);
            return Ok(products);
        }

        [HttpGet("getByIdAsync/{productId}")]
        public async Task<IActionResult> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            return Ok(product);
        }

        [HttpGet("getAllAsync")]
        public async Task<IActionResult> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetAllAsync(cancellationToken);
            return Ok(products);
        }

        [HttpPut("updateAsync")]
        public async Task<IActionResult> UpdateAsync(
            [FromBody] ProductDTO product,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.UpdateAsync(product, cancellationToken);
            return Ok(products);
        }

        [HttpDelete("deleteAsync/{productId}")]
        public async Task<IActionResult> DeleteAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.DeleteAsync(productId, cancellationToken);
            return Ok(result);
        }
    }
}
