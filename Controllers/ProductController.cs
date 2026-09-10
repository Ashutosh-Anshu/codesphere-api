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

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            ProductDTO product,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.CreateAsync(product, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetAllAsync(cancellationToken);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(
            ProductDTO product,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.UpdateAsync(product, cancellationToken);
            return Ok(products);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.DeleteAsync(productId, cancellationToken);
            return Ok(result);
        }
    }
}
