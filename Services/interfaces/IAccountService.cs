using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace codesphere_api.Services.interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<PaginatedResponse<UserDTO>>> GetAllUserAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken);
        Task<ApiResponse<PaginatedResponse<RoleDTO>>> GetAllRoleAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken);
        Task<ApiResponse<List<RoleMenuDTO>>> GetAllRoleMenu(
            CancellationToken cancellationToken);

    }
}
