using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for user-group assignment operations
/// </summary>
public interface IUserGroupManagement
{
    /// <summary>
    /// Assigns a user to groups
    /// </summary>
    Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a user from groups
    /// </summary>
    Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all groups a user is assigned to
    /// </summary>
    Task<IEnumerable<string>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all users assigned to a group
    /// </summary>
    Task<IEnumerable<UserDto>> GetUsersInGroupAsync(string groupId, CancellationToken cancellationToken = default);
}