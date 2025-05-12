using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface IKeycloakService
{
    // User management
    Task<IEnumerable<KeycloakUser>> GetUsersAsync();
    Task<KeycloakUser?> GetUserByIdAsync(string userId);
    Task<KeycloakUser?> GetUserByUsernameAsync(string username);
    Task<bool> CreateUserAsync(KeycloakUser user, string password);
    Task<bool> UpdateUserAsync(string userId, KeycloakUser user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ResetPasswordAsync(string userId, KeycloakCredential credential);
    
    // Role management
    Task<IEnumerable<KeycloakRole>> GetRolesAsync();
    Task<KeycloakRole?> GetRoleByNameAsync(string roleName);
    Task<bool> AssignRolesToUserAsync(string userId, List<string> roles);
    Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roles);
    Task<IEnumerable<KeycloakRole>> GetUserRolesAsync(string userId);
    
    // Group management
    Task<IEnumerable<KeycloakGroup>> GetGroupsAsync();
    Task<KeycloakGroup?> GetGroupByIdAsync(string groupId);
    Task<KeycloakGroup?> GetGroupByPathAsync(string groupPath);
    Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds);
    Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds);
    Task<IEnumerable<string>> GetUserGroupsAsync(string userId);
} 