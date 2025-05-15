using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Interfaces.Identity;

namespace BlogManagementSystem.Application.Services.Identity;

/// <summary>
/// Implementation of IIdentityManager that uses Keycloak as the primary identity provider.
/// </summary>
public class KeycloakIdentityManager(IKeycloakService keycloakService) :
    IIdentityManager,
    IUserManagement,
    IUserRoleManagement,
    IUserGroupManagement,
    IIdentitySynchronization
{
    // Feature support
    public bool SupportsUserCreation => true;
    public bool SupportsDirectRoleCreation => false; // Not implemented in Keycloak service
    public bool SupportsDirectGroupCreation => false; // Not implemented in Keycloak service
    
    // User operations - IUserManagement
    public async Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUsersAsync();
    }
    
    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUserByIdAsync(userId);
    }
    
    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUserByUsernameAsync(username);
    }
    
    public async Task<bool> CreateUserAsync(UserDto user, string password, CancellationToken cancellationToken = default)
    {
        return await keycloakService.CreateUserAsync(user, password);
    }
    
    public async Task<bool> UpdateUserAsync(string userId, UserDto user, CancellationToken cancellationToken = default)
    {
        return await keycloakService.UpdateUserAsync(userId, user);
    }
    
    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await keycloakService.DeleteUserAsync(userId);
    }
    
    public async Task<bool> ResetPasswordAsync(string userId, CredentialDto credential, CancellationToken cancellationToken = default)
    {
        return await keycloakService.ResetPasswordAsync(userId, credential);
    }
    
    // Role operations - IRoleManagement
    public async Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetRolesAsync();
    }
    
    public async Task<RoleDto?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var roles = await keycloakService.GetRolesAsync();
        return roles.FirstOrDefault(r => r.Id == roleId);
    }
    
    public async Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetRoleByNameAsync(roleName);
    }
    
    // These methods are not supported by Keycloak service but required by the interface
    public Task<RoleDto> CreateRoleAsync(RoleDto role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Role creation is not implemented in the current Keycloak service");
    }
    
    public Task<bool> UpdateRoleAsync(string roleId, RoleDto role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Role update is not implemented in the current Keycloak service");
    }
    
    public Task<bool> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Role deletion is not implemented in the current Keycloak service");
    }
    
    // User-Role operations - IUserRoleManagement
    public async Task<bool> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        return await keycloakService.AssignRolesToUserAsync(userId, roleIds);
    }
    
    public async Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        return await keycloakService.RemoveRolesFromUserAsync(userId, roleIds);
    }
    
    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUserRolesAsync(userId);
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        // Get all users and filter those who have the role
        var allUsers = await keycloakService.GetUsersAsync();
        var usersInRole = new List<UserDto>();
        
        foreach (var user in allUsers)
        {
            if (user.Id == null) continue;
            
            var userRoles = await keycloakService.GetUserRolesAsync(user.Id);
            if (userRoles.Any(r => r.Id == roleId))
            {
                usersInRole.Add(user);
            }
        }
        
        return usersInRole;
    }
    
    // Group operations - IGroupManagement
    public async Task<IEnumerable<GroupDto>> GetGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetGroupsAsync();
    }
    
    public async Task<GroupDto?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetGroupByIdAsync(groupId);
    }
    
    public async Task<GroupDto?> GetGroupByPathAsync(string groupPath, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetGroupByPathAsync(groupPath);
    }
    
    // These methods are not supported by Keycloak service but required by the interface
    public Task<GroupDto> CreateGroupAsync(GroupDto group, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group creation is not implemented in the current Keycloak service");
    }
    
    public Task<bool> UpdateGroupAsync(string groupId, GroupDto group, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group update is not implemented in the current Keycloak service");
    }
    
    public Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group deletion is not implemented in the current Keycloak service");
    }
    
    // User-Group operations - IUserGroupManagement
    public async Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default)
    {
        return await keycloakService.AssignUserToGroupsAsync(userId, groupIds);
    }
    
    public async Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default)
    {
        return await keycloakService.RemoveUserFromGroupsAsync(userId, groupIds);
    }
    
    public async Task<IEnumerable<string>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUserGroupsAsync(userId);
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersInGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        // Get all users and filter those who are in the group
        var allUsers = await keycloakService.GetUsersAsync();
        var usersInGroup = new List<UserDto>();
        
        foreach (var user in allUsers)
        {
            if (user.Id == null) continue;
            
            var userGroups = await keycloakService.GetUserGroupsAsync(user.Id);
            if (userGroups.Contains(groupId))
            {
                usersInGroup.Add(user);
            }
        }
        
        return usersInGroup;
    }
    
    // Utilities - IIdentitySynchronization
    public async Task<bool> SynchronizeUsersAsync(CancellationToken cancellationToken = default)
    {
        // No synchronization needed in direct mode
        return true;
    }
    
    // Group-Role operations - IGroupRoleManagement
    // These methods are not supported by Keycloak service but required by the interface
    public Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string groupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group role management is only supported in proxy mode");
    }
    
    public Task<bool> AssignRolesToGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group role management is only supported in proxy mode");
    }
    
    public Task<bool> RemoveRolesFromGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Group role management is only supported in proxy mode");
    }
}