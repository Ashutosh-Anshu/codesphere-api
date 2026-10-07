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

        public async Task<ApiResponse<ProductDTO>> CreateOrUpdateAsync(ProductDTO productDto, CancellationToken cancellationToken = default)
        {
            var product = _mapper.Map<Product>(productDto);
            string msg = string.Empty;
            if (productDto.ProductId == Guid.Empty)
            {
                await _context.Products.AddAsync(product, cancellationToken);
                msg = "Product created successfully";
            }
            else
            {
                product.ModifiedAt = DateTime.UtcNow;
                _context.Products.Update(product);
                msg = "Product updated successfully";
            }
            await _context.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ProductDTO>(product);
            return ApiResponse<ProductDTO>.Ok(
                result,
                msg
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
            {
                return ApiResponse<bool>.Fail(
                    "Invalid product ID",
                    StatusCodes.Status400BadRequest);
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                return ApiResponse<bool>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.Ok(
                true,
                "Product deleted successfully"
            );
        }


        public async Task<ApiResponse<PaginatedResponse<ProductDTO>>> GetAllAsync(
        QueryParameters queryParameters,
        CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchValue))
            {
                var searchValue = queryParameters.SearchValue.Trim();

                query = query.Where(x =>
                    x.Name.Contains(searchValue) ||
                    x.Description.Contains(searchValue));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .OrderByDescending(x => x.ModifiedAt)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(x => new ProductDTO
                {
                    ProductId = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock = x.Stock,
                    ModifiedAt = x.ModifiedAt
                })
                .ToListAsync(cancellationToken);

            var response = new PaginatedResponse<ProductDTO>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            return ApiResponse<PaginatedResponse<ProductDTO>>.Ok(
                response,
                "Products retrieved successfully"
            );
        }


        public async Task<ApiResponse<ProductDTO?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                return ApiResponse<ProductDTO?>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound);
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            return ApiResponse<ProductDTO?>.Ok(
                productDto,
                "Product retrieved successfully"
            );
        }

        public async Task<ApiResponse<ProductDTO>> UpdateAsync(ProductDTO productDto, CancellationToken cancellationToken = default)
        {
            if (productDto.ProductId == Guid.Empty)
            {
                return ApiResponse<ProductDTO>.Fail(
                    "Invalid product ID",
                    StatusCodes.Status400BadRequest);
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productDto.ProductId, cancellationToken);

            if (product == null)
            {
                return ApiResponse<ProductDTO>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound);
            }

            _mapper.Map(productDto, product);
            product.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ProductDTO>(product);
            return ApiResponse<ProductDTO>.Ok(
                result,
                "Product updated successfully",
                StatusCodes.Status200OK);
        }
    }
}
