using AutoMapper;
using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Models;
using codesphere_api.Persistence;
using codesphere_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace codesphere_api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public readonly ApplicationDbContext _context;
        public readonly IMapper _mapper;
        public ProductRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductDTO>> CreateAsync(ProductDTO productDto, CancellationToken cancellationToken = default)
        {
            var product = _mapper.Map<Product>(productDto);
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ProductDTO>(product);
            return new ApiResponse<ProductDTO>(
                true, 
                "Product created successfully", 
                result, 
                StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            if(productId == Guid.Empty)
            {
                return new ApiResponse<bool>(
                    false,
                    "Invalid product ID",
                    false,
                    StatusCodes.Status400BadRequest);
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                return new ApiResponse<bool>(
                    false,
                    "Product not found",
                    false,
                    StatusCodes.Status404NotFound);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return new ApiResponse<bool>(
                true,
                "Product deleted successfully",
                true,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IReadOnlyList<ProductDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.Products.AsNoTracking().ToListAsync(cancellationToken);

            var products = _mapper.Map<IReadOnlyList<ProductDTO>>(result);

            return new ApiResponse<IReadOnlyList<ProductDTO>>(
                true,
                "Products retrieved successfully",
                products,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ProductDTO?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                return new ApiResponse<ProductDTO?>(
                    false,
                    "Product not found",
                    null,
                    StatusCodes.Status404NotFound);
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            return new ApiResponse<ProductDTO?>(
                true,
                "Product retrieved successfully",
                productDto,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ProductDTO>> UpdateAsync(ProductDTO productDto, CancellationToken cancellationToken = default)
        {
            if(productDto.ProductId == Guid.Empty)
            {
                return new ApiResponse<ProductDTO>(
                    false,
                    "Invalid product ID",
                    null,
                    StatusCodes.Status400BadRequest);
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productDto.ProductId, cancellationToken);

            if (product == null)
            {
                return new ApiResponse<ProductDTO>(
                    false,
                    "Product not found",
                    null,
                    StatusCodes.Status404NotFound);
            }

            _mapper.Map(productDto, product);
            product.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ProductDTO>(product);
            return new ApiResponse<ProductDTO>(
                true,
                "Product updated successfully",
                result,
                StatusCodes.Status200OK);
        }
    }
}
