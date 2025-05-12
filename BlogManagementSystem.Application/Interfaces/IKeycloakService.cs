using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces;

public interface IKeycloakService
{
    // User management
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<bool> CreateUserAsync(UserDto user, string password);
    Task<bool> UpdateUserAsync(string userId, UserDto user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ResetPasswordAsync(string userId, CredentialDto credential);
    
    // Role management
    Task<IEnumerable<RoleDto>> GetRolesAsync();
    Task<RoleDto?> GetRoleByNameAsync(string roleName);
    Task<bool> AssignRolesToUserAsync(string userId, List<string> roles);
    Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roles);
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId);
    
    // Group management
    Task<IEnumerable<GroupDto>> GetGroupsAsync();
    Task<GroupDto?> GetGroupByIdAsync(string groupId);
    Task<GroupDto?> GetGroupByPathAsync(string groupPath);
    Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds);
    Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds);
    Task<IEnumerable<string>> GetUserGroupsAsync(string userId);
} 