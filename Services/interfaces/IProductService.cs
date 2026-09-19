using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Models;

namespace codesphere_api.Services.interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDTO>> CreateOrUpdateAsync(
            ProductDTO product,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<ProductDTO?>> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<PaginatedResponse<ProductDTO>>> GetAllAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken);

        Task<ApiResponse<bool>> DeleteAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
