using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for role management operations
/// </summary>
public interface IRoleManagement
{
    /// <summary>
    /// Indicates whether the implementation supports direct role creation
    /// </summary>
    bool SupportsDirectRoleCreation { get; }
    
    /// <summary>
    /// Gets all roles
    /// </summary>
    Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a role by ID
    /// </summary>
    Task<RoleDto?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new role
    /// </summary>
    Task<RoleDto> CreateRoleAsync(RoleDto role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing role
    /// </summary>
    Task<bool> UpdateRoleAsync(string roleId, RoleDto role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a role
    /// </summary>
    Task<bool> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);
}