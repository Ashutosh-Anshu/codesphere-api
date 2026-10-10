using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Models;
using codesphere_api.Services;
using codesphere_api.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace codesphere_api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("getAllUsersAsync")]
        public async Task<IActionResult> GetAllUsersAsync(
            [FromQuery] QueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            var users = await _accountService.GetAllUsersAsync(
                queryParameters,
                cancellationToken);

            return Ok(users);
        }

        [HttpGet("getAllUserRoles")]
        public async Task<IActionResult> GetAllUserRoles(
            CancellationToken cancellationToken = default)
        {
            var users = await _accountService.GetAllUserRoles(cancellationToken);

            return Ok(users);
        }

        [HttpGet("getAllRoleAsync")]
        public async Task<IActionResult> GetAllRoleAsync(
            [FromQuery] QueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            var roles = await _accountService.GetAllRoleAsync(queryParameters, cancellationToken);

            return Ok(roles);
        }

        [HttpGet("getAllRoleMenu")]
        public async Task<IActionResult> GetAllRoleMenu(CancellationToken cancellationToken = default)
        {
            var users = await _accountService.GetAllRoleMenu(cancellationToken);
            return Ok(users);
        }

        [HttpPost("createOrUpdateRoleAsync")]
        public async Task<IActionResult> CreateOrUpdateRoleAsync(
            [FromBody] RoleDetailDTO roleDetail,
            CancellationToken cancellationToken = default)
        {
            var roles = await _accountService.CreateOrUpdateRoleAsync(roleDetail, cancellationToken);
            return Ok(roles);
        }

        [HttpGet("getRoleById/{roleId}")]
        public async Task<IActionResult> GetRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var role = await _accountService.GetRoleByIdAsync(roleId, cancellationToken);
            return Ok(role);
        }

        [HttpDelete("deleteRoleById/{roleId}")]
        public async Task<IActionResult> DeleteRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var result = await _accountService.DeleteRoleByIdAsync(roleId, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("deleteUserById/{userId}")]
        public async Task<IActionResult> DeleteUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _accountService.DeleteUserByIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("createOrUpdateUserAsync")]
        public async Task<IActionResult> CreateOrUpdateUserAsync(
            [FromBody] UserDetailDTO userDetail,
            CancellationToken cancellationToken = default)
        {
            var users = await _accountService.CreateOrUpdateUserAsync(userDetail, cancellationToken);
            return Ok(users);
        }


        [HttpGet("getUserById/{userId}")]
        public async Task<IActionResult> GetUserById(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var role = await _accountService.GetUserByIdAsync(userId, cancellationToken);
            return Ok(role);
        }

        [HttpPost("loginAsync")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest response,
            CancellationToken cancellationToken = default)
        {
            var roles = await _accountService.LoginAsync(response, cancellationToken);
            return Ok(roles);
        }

        [HttpGet("getMenusByUserId/{userId}")]
        public async Task<IActionResult> GetMenusByUserId(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var role = await _accountService.GetMenusByUserId(userId, cancellationToken);
            return Ok(role);
        }
    }
}
