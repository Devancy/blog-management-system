using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces;

/// <summary>
/// Abstract interface for identity management operations that works in both modes:
/// 1. Keycloak as primary IDP (direct management of users, roles, and groups in Keycloak)
/// 2. Keycloak as IDP Proxy (users from MS Entra ID, local management of roles and groups)
/// </summary>
public interface IIdentityManager
{
    // Properties to indicate supported features
    bool SupportsUserCreation { get; }
    bool SupportsDirectRoleCreation { get; }
    bool SupportsDirectGroupCreation { get; }
    
    // User operations
    Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> CreateUserAsync(UserDto user, string password, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(string userId, UserDto user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(string userId, CredentialDto credential, CancellationToken cancellationToken = default);
    
    // Role operations
    Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);
    Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateRoleAsync(RoleDto role, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(string roleId, RoleDto role, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);
    Task<bool> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default);
    Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId, CancellationToken cancellationToken = default);
    
    // Group operations
    Task<IEnumerable<GroupDto>> GetGroupsAsync(CancellationToken cancellationToken = default);
    Task<GroupDto?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default);
    Task<GroupDto?> GetGroupByPathAsync(string groupPath, CancellationToken cancellationToken = default);
    Task<GroupDto> CreateGroupAsync(GroupDto group, CancellationToken cancellationToken = default);
    Task<bool> UpdateGroupAsync(string groupId, GroupDto group, CancellationToken cancellationToken = default);
    Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default);
    Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default);
    Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersInGroupAsync(string groupId, CancellationToken cancellationToken = default);
    
    // Group-Role operations
    Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string groupId, CancellationToken cancellationToken = default);
    Task<bool> AssignRolesToGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default);
    Task<bool> RemoveRolesFromGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default);
    
    // Utilities
    Task<bool> SynchronizeUsersAsync(CancellationToken cancellationToken = default);
} 