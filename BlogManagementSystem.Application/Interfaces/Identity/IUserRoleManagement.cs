using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for user-role assignment operations
/// </summary>
public interface IUserRoleManagement
{
    /// <summary>
    /// Assigns roles to a user
    /// </summary>
    Task<bool> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes roles from a user
    /// </summary>
    Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all users assigned to a role
    /// </summary>
    Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId, CancellationToken cancellationToken = default);
}