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

    }
}
