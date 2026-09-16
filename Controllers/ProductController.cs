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

        [HttpPost("createOrUpdateAsync")]
        public async Task<IActionResult> CreateOrUpdateAsync(
            [FromBody] ProductDTO product,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.CreateOrUpdateAsync(product, cancellationToken);
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
