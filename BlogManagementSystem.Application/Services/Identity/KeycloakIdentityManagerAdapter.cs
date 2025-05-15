using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Application.Interfaces;

namespace BlogManagementSystem.Application.Services.Identity;

/// <summary>
/// Adapter for KeycloakIdentityManager that safely handles unsupported operations
/// by providing meaningful fallbacks instead of throwing exceptions.
/// </summary>
public class KeycloakIdentityManagerAdapter(KeycloakIdentityManager keycloakManager) : IIdentityManager
{
    // Feature support properties - delegate directly
    public bool SupportsUserCreation => keycloakManager.SupportsUserCreation;
    public bool SupportsDirectRoleCreation => keycloakManager.SupportsDirectRoleCreation;
    public bool SupportsDirectGroupCreation => keycloakManager.SupportsDirectGroupCreation;

    // User operations - delegate directly as they are supported
    public Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default) => 
        keycloakManager.GetUsersAsync(cancellationToken);

    public Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUserByIdAsync(userId, cancellationToken);

    public Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUserByUsernameAsync(username, cancellationToken);

    public Task<bool> CreateUserAsync(UserDto user, string password, CancellationToken cancellationToken = default) => 
        keycloakManager.CreateUserAsync(user, password, cancellationToken);

    public Task<bool> UpdateUserAsync(string userId, UserDto user, CancellationToken cancellationToken = default) => 
        keycloakManager.UpdateUserAsync(userId, user, cancellationToken);

    public Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default) => 
        keycloakManager.DeleteUserAsync(userId, cancellationToken);

    public Task<bool> ResetPasswordAsync(string userId, CredentialDto credential, CancellationToken cancellationToken = default) => 
        keycloakManager.ResetPasswordAsync(userId, credential, cancellationToken);

    // Role operations - some are supported, some need safe fallbacks
    public Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default) => 
        keycloakManager.GetRolesAsync(cancellationToken);

    public Task<RoleDto?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetRoleByIdAsync(roleId, cancellationToken);

    public Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default) => 
        keycloakManager.GetRoleByNameAsync(roleName, cancellationToken);

    // Unsupported operation with fallback
    public Task<RoleDto> CreateRoleAsync(RoleDto role, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new RoleDto { Id = string.Empty, Name = role.Name, Description = "Not created - operation not supported" });
    }

    public Task<bool> UpdateRoleAsync(string roleId, RoleDto role, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default) => 
        keycloakManager.AssignRolesToUserAsync(userId, roleIds, cancellationToken);

    public Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default) => 
        keycloakManager.RemoveRolesFromUserAsync(userId, roleIds, cancellationToken);

    public Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUserRolesAsync(userId, cancellationToken);

    public Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUsersInRoleAsync(roleId, cancellationToken);

    // Group operations - delegate directly for supported operations
    public Task<IEnumerable<GroupDto>> GetGroupsAsync(CancellationToken cancellationToken = default) => 
        keycloakManager.GetGroupsAsync(cancellationToken);

    public Task<GroupDto?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetGroupByIdAsync(groupId, cancellationToken);

    public Task<GroupDto?> GetGroupByPathAsync(string groupPath, CancellationToken cancellationToken = default) => 
        keycloakManager.GetGroupByPathAsync(groupPath, cancellationToken);

    public Task<GroupDto> CreateGroupAsync(GroupDto group, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GroupDto { Id = string.Empty, Name = group.Name, Path = group.Path });
    }

    public Task<bool> UpdateGroupAsync(string groupId, GroupDto group, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default) => 
        keycloakManager.AssignUserToGroupsAsync(userId, groupIds, cancellationToken);

    public Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default) => 
        keycloakManager.RemoveUserFromGroupsAsync(userId, groupIds, cancellationToken);

    public Task<IEnumerable<string>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUserGroupsAsync(userId, cancellationToken);

    public Task<IEnumerable<UserDto>> GetUsersInGroupAsync(string groupId, CancellationToken cancellationToken = default) => 
        keycloakManager.GetUsersInGroupAsync(groupId, cancellationToken);

    // Group-Role operations - provide safe fallbacks
    public Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string groupId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<RoleDto>>(new List<RoleDto>());
    }

    public Task<bool> AssignRolesToGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> RemoveRolesFromGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    // Utilities
    public Task<bool> SynchronizeUsersAsync(CancellationToken cancellationToken = default) => 
        keycloakManager.SynchronizeUsersAsync(cancellationToken);
}