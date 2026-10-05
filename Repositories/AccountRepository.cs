using AutoMapper;
using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Models;
using codesphere_api.Persistence;
using codesphere_api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace codesphere_api.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountRepository(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<PaginatedResponse<UserDTO>>> GetAllUserAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchValue))
            {
                var searchValue = queryParameters.SearchValue.Trim();

                query = query.Where(x =>
                    x.FirstName.Contains(searchValue) ||
                    x.LastName.Contains(searchValue) ||
                    (x.Email != null && x.Email.Contains(searchValue)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(x => x.UpdatedAt)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(x => new
                {
                    User = x,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == x.Id)
                        .Join(
                            _context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => new
                            {
                                RoleId = r.Id,
                                RoleName = r.Name
                            })
                        .FirstOrDefault()
                })
                .Select(x => new UserDTO
                {
                    UserId = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email ?? string.Empty,
                    Username = x.User.UserName ?? string.Empty,

                    RoleId = x.Role != null
                        ? x.Role.RoleId
                        : Guid.Empty,

                    RoleName = x.Role != null
                        ? x.Role.RoleName ?? string.Empty
                        : string.Empty,

                    IsActive = x.User.IsActive,
                    IsSystem = x.User.IsSystem,
                    UpdatedAt = x.User.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var response = new PaginatedResponse<UserDTO>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            return new ApiResponse<PaginatedResponse<UserDTO>>(
                true,
                "Users retrieved successfully",
                response,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PaginatedResponse<RoleDTO>>> GetAllRoleAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken)
        {
            var query = _roleManager.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchValue))
            {
                var searchValue = queryParameters.SearchValue.Trim();

                query = query.Where(x =>
                    (x.Name != null && x.Name.Contains(searchValue)) ||
                    (x.Description != null && x.Description.Contains(searchValue)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var roles = await query
                .OrderByDescending(x => x.UpdatedAt)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(x => new RoleDTO
                {
                    RoleId = x.Id,
                    Name = x.Name ?? string.Empty,
                    Description = x.Description ?? string.Empty,
                    IsActive = x.IsActive,
                    IsSystem = x.IsSystem,
                    UpdatedAt = x.UpdatedAt ?? x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var response = new PaginatedResponse<RoleDTO>
            {
                Items = roles,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            return new ApiResponse<PaginatedResponse<RoleDTO>>(
                true,
                "Roles retrieved successfully",
                response,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<List<RoleMenuDTO>>> GetAllRoleMenu(
            CancellationToken cancellationToken)
        {
            var menus = await _context.Menus
                .AsNoTracking()
                .Select(x => new RoleMenuDTO
                {
                    MenuId = x.Id,
                    MenuName = x.Name,
                    Description = x.Description,

                    Permissions = x.Permissions
                        .Select(p => new PermissionDTO
                        {
                            PermissionId = p.Id,
                            PermissionName = p.Code
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<List<RoleMenuDTO>>(
                success: true,
                message: "Role menus retrieved successfully.",
                data: menus);
        }
        public async Task<ApiResponse<bool>> CreateOrUpdateRoleAsync(
            RoleDetailDTO roleDetail,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ApplicationRole? role;

                if (roleDetail.RoleId == Guid.Empty)
                {
                    role = _mapper.Map<ApplicationRole>(roleDetail);
                    role.Id = Guid.NewGuid();
                    role.CreatedAt = DateTime.UtcNow;
                    var result = await _roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(x => x.Description));

                        return new ApiResponse<bool>(
                            false,
                            errors,
                            false,
                            StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    role = await _roleManager.FindByIdAsync(roleDetail.RoleId.ToString());

                    if (role == null)
                    {
                        return new ApiResponse<bool>(
                            false,
                            "Role not found.",
                            false,
                            StatusCodes.Status404NotFound);
                    }

                    _mapper.Map(roleDetail, role);
                    role.UpdatedAt = DateTime.UtcNow;
                    var result = await _roleManager.UpdateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(x => x.Description));

                        return new ApiResponse<bool>(
                            false,
                            errors,
                            false,
                            StatusCodes.Status400BadRequest);
                    }

                    await _context.RolePermissions
                        .Where(x => x.RoleId == role.Id)
                        .ExecuteDeleteAsync(cancellationToken);
                }

                var permissions = roleDetail.Permissions?
                    .Where(x => x.IsAllowed)
                    .Select(x => new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = x.PermissionId
                    })
                    .ToList()
                    ?? new List<RolePermission>();

                if (permissions.Count > 0)
                {
                    await _context.RolePermissions.AddRangeAsync(
                        permissions,
                        cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<bool>(
                    true,
                    "Role created or updated successfully.",
                    true,
                    StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return new ApiResponse<bool>(
                    false,
                    "An error occurred while saving the role.",
                    false,
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ApiResponse<RoleDetailDTO?>> GetRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var role = await _roleManager.Roles
                .Include(x => x.RolePermissions)
                .ThenInclude(m => m.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);

            if (role == null)
            {
                return new ApiResponse<RoleDetailDTO?>(
                    false,
                    "Role not found.",
                    null,
                    StatusCodes.Status404NotFound);
            }

            var roleDetail = new RoleDetailDTO
            {
                RoleId = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description ?? string.Empty,
                IsActive = role.IsActive,
                IsSystem = role.IsSystem,
                UpdatedAt = role.UpdatedAt,

                Permissions = role.RolePermissions
                    .Select(x => new MenuPermissionDTO
                    {
                        MenuId = x.Permission.MenuId,
                        PermissionId = x.PermissionId,
                        IsAllowed = true
                    })
                    .ToList()
            };

            return new ApiResponse<RoleDetailDTO?>(
                true,
                "Role retrieved successfully.",
                roleDetail,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> DeleteRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            if (roleId == Guid.Empty)
            {
                return new ApiResponse<bool>(
                    false,
                    "Invalid role ID",
                    false,
                    StatusCodes.Status400BadRequest);
            }
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);

            if (role == null)
            {
                return new ApiResponse<bool>(
                    false,
                    "Role not found",
                    false,
                    StatusCodes.Status404NotFound);
            }
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            return new ApiResponse<bool>(
                true,
                "Role deleted successfully",
                true,
                StatusCodes.Status200OK);
        }
    }

}
