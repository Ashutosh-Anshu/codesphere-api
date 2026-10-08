using AutoMapper;
using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using codesphere_api.Models;
using codesphere_api.Persistence;
using codesphere_api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ApiResponse<PaginatedResponse<UserDTO>>> GetAllUsersAsync(
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
                    RoleId = x.Role != null
                        ? x.Role.RoleId
                        : Guid.Empty,

                    RoleName = x.Role != null
                        ? x.Role.RoleName ?? string.Empty
                        : string.Empty,

                    IsActive = x.User.IsActive,
                    IsSystem = x.User.IsSystem,
                    UpdatedAt = x.User.UpdatedAt ?? x.User.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var response = new PaginatedResponse<UserDTO>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            return ApiResponse<PaginatedResponse<UserDTO>>.Ok(
                response,
                "Users retrieved successfully"
            );
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

            return ApiResponse<PaginatedResponse<RoleDTO>>.Ok(
                response,
                "Roles retrieved successfully"
            );
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

            return ApiResponse<List<RoleMenuDTO>>.Ok(
                    menus,
                 "Role menus retrieved successfully."
                 );
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

                        return ApiResponse<bool>.Fail(
                            errors,
                            StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    role = await _roleManager.FindByIdAsync(roleDetail.RoleId.ToString());

                    if (role == null)
                    {
                        return ApiResponse<bool>.Fail(
                            "Role not found.",
                            StatusCodes.Status404NotFound);
                    }

                    _mapper.Map(roleDetail, role);
                    role.UpdatedAt = DateTime.UtcNow;
                    var result = await _roleManager.UpdateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(x => x.Description));

                        return ApiResponse<bool>.Fail(
                            errors,
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

                return ApiResponse<bool>.Ok(
                    true,
                    "Role created or updated successfully."
                );
            }
            catch (Exception)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while saving the role.",
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
                return ApiResponse<RoleDetailDTO?>.Fail(
                    "Role not found.",
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

            return ApiResponse<RoleDetailDTO?>.Ok(
                roleDetail,
                "Role retrieved successfully.",
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<bool>> DeleteRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            if (roleId == Guid.Empty)
            {
                return ApiResponse<bool>.Fail(
                    "Invalid role ID",
                    StatusCodes.Status400BadRequest);
            }
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);

            if (role == null)
            {
                return ApiResponse<bool>.Fail(
                    "Role not found",
                    StatusCodes.Status404NotFound);
            }
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(
                true,
                 "Role deleted successfully",
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> DeleteUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                return ApiResponse<bool>.Fail(
                    "Invalid user ID",
                    StatusCodes.Status400BadRequest);
            }
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ApiResponse<bool>.Fail(
                    "User not found",
                    StatusCodes.Status404NotFound);
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var errors = deleteResult.Errors.Select(x => x.Description).ToList();
                return ApiResponse<bool>.Fail("User deletion failed.", StatusCodes.Status400BadRequest, errors);
            }

            return ApiResponse<bool>.Ok(
                true,
                "User deleted successfully",
                StatusCodes.Status200OK);
        }

        public async Task<List<RoleItemDTO>> GetAllUserRoles(
            CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .Select(x => new RoleItemDTO
                {
                    RoleId = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<ApiResponse<bool>> CreateOrUpdateUserAsync(
            UserDetailDTO userDetail,
            CancellationToken cancellationToken = default)
        {
            ApplicationUser? user;

            if (userDetail.UserId == Guid.Empty)
            {
                if (!string.IsNullOrWhiteSpace(userDetail.Email))
                {
                    var existingUser = await _userManager.FindByEmailAsync(userDetail.Email);
                    if (existingUser != null)
                    {
                        return ApiResponse<bool>.Fail(
                            "Email address is already in use.",
                            StatusCodes.Status400BadRequest);
                    }
                }

                user = _mapper.Map<ApplicationUser>(userDetail);
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.UtcNow;
                user.UserName = user.Email;

                var createResult = await _userManager.CreateAsync(user, userDetail.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(x => x.Description).ToList();
                    return ApiResponse<bool>.Fail("User creation failed.", StatusCodes.Status400BadRequest, errors);
                }
            }
            else
            {
                user = await _userManager.FindByIdAsync(userDetail.UserId.ToString());
                if (user == null)
                {
                    return ApiResponse<bool>.Fail("User not found.", StatusCodes.Status404NotFound);
                }

                if (!string.IsNullOrWhiteSpace(userDetail.Email) && user.Email != userDetail.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(userDetail.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        return ApiResponse<bool>.Fail("Email address is already in use.", StatusCodes.Status400BadRequest);
                    }
                }

                _mapper.Map(userDetail, user);
                user.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(x => x.Description).ToList();
                    return ApiResponse<bool>.Fail("User update failed.", StatusCodes.Status400BadRequest, errors);
                }
            }

            var roleId = userDetail.RoleId.ToString() ?? Guid.Empty.ToString();

            if (roleId == Guid.Empty.ToString())
            {
                return ApiResponse<bool>.Fail("Role not found.", StatusCodes.Status404NotFound);
            }

            var newRole = await _roleManager.FindByIdAsync(roleId);
            if (newRole == null)
            {
                return ApiResponse<bool>.Fail("Role not found.", StatusCodes.Status404NotFound);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, newRole.Name ?? string.Empty);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(x => x.Description).ToList();
                return ApiResponse<bool>.Fail("Role assignment failed.", StatusCodes.Status400BadRequest, errors);
            }

            return ApiResponse<bool>.Ok(
                true,
                "User created or updated successfully.",
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<UserDetailDTO?>> GetUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                return ApiResponse<UserDetailDTO?>.Fail(
                    "Invalid user ID.",
                    StatusCodes.Status400BadRequest);
            }

            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Id == userId,
                    cancellationToken);

            if (user == null)
            {
                return ApiResponse<UserDetailDTO?>.Fail(
                    "User not found.",
                    StatusCodes.Status404NotFound);
            }

            var roles = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == userId
                select role
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

            var userDetail = _mapper.Map<UserDetailDTO>(user);


            userDetail.RoleId = roles?.Id ?? Guid.Empty;
            userDetail.UserId = user.Id;
            userDetail.Email = user.Email ?? string.Empty;
            userDetail.Password = user.PasswordHash ?? string.Empty;

            return ApiResponse<UserDetailDTO?>.Ok(
                userDetail,
                "User retrieved successfully.",
                StatusCodes.Status200OK);
        }

    }

}
