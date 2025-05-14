using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Application.Extensions;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Services.Identity;

/// <summary>
/// Implementation of IIdentityManager that uses local repositories for users, roles, and groups.
/// </summary>
public class ProxyIdentityManager(
    ILocalUserIdentityRepository userRepository,
    ILocalRoleRepository roleRepository,
    ILocalGroupRepository groupRepository)
    : IIdentityManager
{
    // Feature support
    public bool SupportsUserCreation => true; // Users are managed locally now
    public bool SupportsDirectRoleCreation => true; // Roles are managed locally
    public bool SupportsDirectGroupCreation => true; // Groups are managed locally
    
    // User operations - Now using local repository
    public async Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(u => u.ToDto());
    }
    
    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        return user != null ? user.ToDto() : null;
    }
    
    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
        return user != null ? user.ToDto() : null;
    }
    
    public async Task<bool> CreateUserAsync(UserDto user, string password, CancellationToken cancellationToken = default)
    {
        var newUser = new LocalUserIdentity
        {
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            IsEnabled = user.Enabled,
            CreatedAt = DateTime.UtcNow
        };
        
        // Store password securely if your repository supports it
        
        await userRepository.CreateAsync(newUser, cancellationToken);
        return true;
    }
    
    public async Task<bool> UpdateUserAsync(string userId, UserDto user, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (existingUser == null)
            return false;
            
        existingUser.Username = user.Username ?? existingUser.Username;
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.FirstName = user.FirstName ?? existingUser.FirstName;
        existingUser.LastName = user.LastName ?? existingUser.LastName;
        existingUser.IsEnabled = user.Enabled;
        existingUser.UpdatedAt = DateTime.UtcNow;
        
        await userRepository.UpdateAsync(existingUser, cancellationToken);
        return true;
    }
    
    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteAsync(userId, cancellationToken);
        return true;
    }
    
    public async Task<bool> ResetPasswordAsync(string userId, CredentialDto credential, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return false;
            
        // Implement password reset logic based on your authentication system
        // This might involve hashing the password and updating it in the repository
        
        return true;
    }
    
    // Role operations - These manage roles locally
    public async Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(r => new RoleDto
        {
            Id = r.Id.ToString(),
            Name = r.Name,
            Description = r.Description
        });
    }
    
    public async Task<RoleDto?> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(roleId, out var guid))
            return null;
            
        var role = await roleRepository.GetByIdAsync(guid, cancellationToken);
        if (role == null)
            return null;
            
        return new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name,
            Description = role.Description
        };
    }
    
    public async Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetByNameAsync(roleName, cancellationToken);
        if (role == null)
            return null;
            
        return new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name,
            Description = role.Description
        };
    }
    
    public async Task<RoleDto> CreateRoleAsync(RoleDto role, CancellationToken cancellationToken = default)
    {
        var newRole = new LocalRole
        {
            Name = role.Name ?? string.Empty,
            Description = role.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await roleRepository.CreateAsync(newRole, cancellationToken);
        
        return new RoleDto
        {
            Id = created.Id.ToString(),
            Name = created.Name,
            Description = created.Description
        };
    }
    
    public async Task<bool> UpdateRoleAsync(string roleId, RoleDto role, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(roleId, out var guid))
            return false;
            
        var existingRole = await roleRepository.GetByIdAsync(guid, cancellationToken);
        if (existingRole == null)
            return false;
            
        existingRole.Name = role.Name ?? existingRole.Name;
        existingRole.Description = role.Description ?? existingRole.Description;
        existingRole.UpdatedAt = DateTime.UtcNow;
        
        await roleRepository.UpdateAsync(existingRole, cancellationToken);
        return true;
    }
    
    public async Task<bool> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(roleId, out var guid))
            return false;
            
        await roleRepository.DeleteAsync(guid, cancellationToken);
        return true;
    }
    
    public async Task<bool> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        foreach (var roleIdStr in roleIds)
        {
            if (!Guid.TryParse(roleIdStr, out var roleId))
                continue;
                
            await roleRepository.AddUserToRoleAsync(userId, roleId, cancellationToken);
        }
        
        return true;
    }
    
    public async Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        foreach (var roleIdStr in roleIds)
        {
            if (!Guid.TryParse(roleIdStr, out var roleId))
                continue;
                
            await roleRepository.RemoveUserFromRoleAsync(userId, roleId, cancellationToken);
        }
        
        return true;
    }
    
    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetByUserIdAsync(userId, cancellationToken);
        
        return roles.Select(r => new RoleDto
        {
            Id = r.Id.ToString(),
            Name = r.Name,
            Description = r.Description
        });
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(roleId, out var guid))
            return [];
            
        // Get user IDs in the role
        var userIds = await roleRepository.GetUserIdsInRoleAsync(guid, cancellationToken);
        
        // Get user details from local repository
        var users = new List<UserDto>();
        foreach (var userId in userIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                users.Add(user.ToDto());
            }
        }
        
        return users;
    }
    
    // Group operations - These manage groups locally
    public async Task<IEnumerable<GroupDto>> GetGroupsAsync(CancellationToken cancellationToken = default)
    {
        var rootGroups = await groupRepository.GetAllAsync(cancellationToken);
        return rootGroups.ToHierarchicalDto();
    }
    
    public async Task<GroupDto?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return null;
            
        var group = await groupRepository.GetByIdAsync(guid, cancellationToken);
        if (group == null)
            return null;
            
        return group.ToDto();
    }
    
    public async Task<GroupDto?> GetGroupByPathAsync(string groupPath, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetByPathAsync(groupPath, cancellationToken);
        if (group == null)
            return null;
            
        return group.ToDto();
    }
    
    public async Task<GroupDto> CreateGroupAsync(GroupDto group, CancellationToken cancellationToken = default)
    {
        // Determine parent path and generate new path
        string path;
        if (!string.IsNullOrEmpty(group.ParentGroupId) && Guid.TryParse(group.ParentGroupId, out var parentId))
        {
            var parentGroup = await groupRepository.GetByIdAsync(parentId, cancellationToken);
            if (parentGroup != null)
            {
                path = parentGroup.Path == "/" ? $"/{group.Name}" : $"{parentGroup.Path}/{group.Name}";
            }
            else
            {
                path = $"/{group.Name}";
            }
        }
        else
        {
            path = $"/{group.Name}";
        }
        
        var newGroup = new LocalGroup
        {
            Name = group.Name ?? string.Empty,
            Path = path,
            ParentGroupId = !string.IsNullOrEmpty(group.ParentGroupId) && Guid.TryParse(group.ParentGroupId, out var pgid) ? pgid : null,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await groupRepository.CreateAsync(newGroup, cancellationToken);
        
        return created.ToDto();
    }
    
    public async Task<bool> UpdateGroupAsync(string groupId, GroupDto group, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return false;
            
        var existingGroup = await groupRepository.GetByIdAsync(guid, cancellationToken);
        if (existingGroup == null)
            return false;
            
        // Handle path updates if the name changes
        if (group.Name != existingGroup.Name)
        {
            // Update the path of this group and all subgroups
            var parentPath = existingGroup.ParentGroupId.HasValue
                ? (await groupRepository.GetByIdAsync(existingGroup.ParentGroupId.Value, cancellationToken))?.Path ?? "/"
                : "/";
                
            var newPath = $"{parentPath}/{group.Name}";
            if (parentPath == "/")
                newPath = $"/{group.Name}";
                
            existingGroup.Path = newPath;
            
            // TODO: Update paths of all child groups (recursive)
        }
        
        existingGroup.Name = group.Name ?? existingGroup.Name;
        existingGroup.UpdatedAt = DateTime.UtcNow;
        
        await groupRepository.UpdateAsync(existingGroup, cancellationToken);
        return true;
    }
    
    public async Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return false;
            
        await groupRepository.DeleteAsync(guid, cancellationToken);
        return true;
    }
    
    public async Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default)
    {
        foreach (var groupIdStr in groupIds)
        {
            if (!Guid.TryParse(groupIdStr, out var groupId))
                continue;
                
            await groupRepository.AddUserToGroupAsync(userId, groupId, cancellationToken);
        }
        
        return true;
    }
    
    public async Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default)
    {
        foreach (var groupIdStr in groupIds)
        {
            if (!Guid.TryParse(groupIdStr, out var groupId))
                continue;
                
            await groupRepository.RemoveUserFromGroupAsync(userId, groupId, cancellationToken);
        }
        
        return true;
    }
    
    public async Task<IEnumerable<string>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var groups = await groupRepository.GetByUserIdAsync(userId, cancellationToken);
        return groups.Select(g => g.Id.ToString());
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersInGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return [];
            
        // Get user IDs in the group
        var userIds = await groupRepository.GetUserIdsInGroupAsync(guid, cancellationToken);
        
        // Get user details from local repository
        var users = new List<UserDto>();
        foreach (var userId in userIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                users.Add(user.ToDto());
            }
        }
        
        return users;
    }
    
    // Group-Role operations
    public async Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string groupId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return [];
        
        // Get the group to find its path
        var group = await groupRepository.GetByIdAsync(guid, cancellationToken);
        if (group == null)
            return [];
        
        // Get roles for this group path
        var roles = await roleRepository.GetByGroupPathAsync(group.Path, cancellationToken);
        
        return roles.Select(r => new RoleDto
        {
            Id = r.Id.ToString(),
            Name = r.Name,
            Description = r.Description
        });
    }
    
    public async Task<bool> AssignRolesToGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return false;
        
        foreach (var roleIdStr in roleIds)
        {
            if (!Guid.TryParse(roleIdStr, out var roleId))
                continue;
            
            await roleRepository.AssignRoleToGroupAsync(roleId, guid, cancellationToken);
        }
        
        return true;
    }
    
    public async Task<bool> RemoveRolesFromGroupAsync(string groupId, List<string> roleIds, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(groupId, out var guid))
            return false;
        
        foreach (var roleIdStr in roleIds)
        {
            if (!Guid.TryParse(roleIdStr, out var roleId))
                continue;
            
            await roleRepository.RemoveRoleFromGroupAsync(roleId, guid, cancellationToken);
        }
        
        return true;
    }
    
    // Utilities
    public async Task<bool> SynchronizeUsersAsync(CancellationToken cancellationToken = default)
    {
        // This method might now synchronize with an external system or perform cleanup
        return true;
    }
} 