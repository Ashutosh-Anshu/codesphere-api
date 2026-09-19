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
        public Task<ApiResponse<ProductDTO>> CreateOrUpdateAsync(ProductDTO product, CancellationToken cancellationToken = default)
        {
            return _productRepository.CreateOrUpdateAsync(product, cancellationToken);
        }

        public Task<ApiResponse<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _productRepository.DeleteAsync(productId, cancellationToken);
        }

        public Task<ApiResponse<PaginatedResponse<ProductDTO>>> GetAllAsync(QueryParameters queryParameters, CancellationToken cancellationToken = default)
        {
            return _productRepository.GetAllAsync(queryParameters, cancellationToken);
        }

        public Task<ApiResponse<ProductDTO?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _productRepository.GetByIdAsync(productId, cancellationToken);
        }

    }
}
