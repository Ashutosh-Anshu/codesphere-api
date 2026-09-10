using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Repositories.Interfaces;
using codesphere_api.Services.interfaces;

namespace codesphere_api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public Task<ApiResponse<ProductDTO>> CreateAsync(ProductDTO product, CancellationToken cancellationToken = default)
        {
            return _productRepository.CreateAsync(product, cancellationToken);
        }

        public Task<ApiResponse<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _productRepository.DeleteAsync(productId, cancellationToken);
        }

        public Task<ApiResponse<IReadOnlyList<ProductDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _productRepository.GetAllAsync(cancellationToken);
        }

        public Task<ApiResponse<ProductDTO?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _productRepository.GetByIdAsync(productId, cancellationToken);
        }

        public Task<ApiResponse<ProductDTO>> UpdateAsync(ProductDTO product, CancellationToken cancellationToken = default)
        {
            return _productRepository.UpdateAsync(product, cancellationToken);
        }
    }
}
