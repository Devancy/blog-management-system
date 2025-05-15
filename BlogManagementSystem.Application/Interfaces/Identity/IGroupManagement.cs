using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for group management operations
/// </summary>
public interface IGroupManagement
{
    /// <summary>
    /// Indicates whether the implementation supports direct group creation
    /// </summary>
    bool SupportsDirectGroupCreation { get; }
    
    /// <summary>
    /// Gets all groups
    /// </summary>
    Task<IEnumerable<GroupDto>> GetGroupsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a group by ID
    /// </summary>
    Task<GroupDto?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a group by path
    /// </summary>
    Task<GroupDto?> GetGroupByPathAsync(string groupPath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new group
    /// </summary>
    Task<GroupDto> CreateGroupAsync(GroupDto group, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing group
    /// </summary>
    Task<bool> UpdateGroupAsync(string groupId, GroupDto group, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a group
    /// </summary>
    Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default);
}