using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Repositories.Interfaces;
using codesphere_api.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace codesphere_api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<PaginatedResponse<UserDTO>>> GetAllUserAsync(
            QueryParameters queryParameters, 
            CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAllUserAsync(queryParameters, cancellationToken);
        }
        public async Task<ApiResponse<PaginatedResponse<RoleDTO>>> GetAllRoleAsync(
            QueryParameters queryParameters, 
            CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAllRoleAsync(queryParameters, cancellationToken);
        }

        public async Task<ApiResponse<List<RoleMenuDTO>>> GetAllRoleMenu(CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAllRoleMenu(cancellationToken);
        }
        public async Task<ApiResponse<bool>> CreateOrUpdateRoleAsync(
            RoleDetailDTO roleDetail, 
            CancellationToken cancellationToken = default)
        {
            return await _accountRepository.CreateOrUpdateRoleAsync(roleDetail, cancellationToken);
        }
        public async Task<ApiResponse<bool>> CreateOrUpdateUserAsync(
            UserDetailDTO userDetail, 
            CancellationToken cancellationToken = default)
        {
            return await _accountRepository.CreateOrUpdateUserAsync(userDetail, cancellationToken);
        }

        public async Task<ApiResponse<RoleDetailDTO?>> GetRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            return await _accountRepository.GetRoleByIdAsync(roleId, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _accountRepository.DeleteRoleByIdAsync(roleId, cancellationToken);

        }

        public async Task<List<RoleItemDTO>> GetAllUserRoles(CancellationToken cancellationToken = default)
        {
            return await _accountRepository.GetAllUserRoles(cancellationToken);
        }
    }
}
