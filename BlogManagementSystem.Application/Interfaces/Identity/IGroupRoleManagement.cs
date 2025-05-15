using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for group-role assignment operations
/// </summary>
public interface IGroupRoleManagement
{
    /// <summary>
    /// Gets all roles assigned to a group
    /// </summary>
    Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Assigns roles to a group
    /// </summary>
    Task<bool> AssignRolesToGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes roles from a group
    /// </summary>
    Task<bool> RemoveRolesFromGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default);
}