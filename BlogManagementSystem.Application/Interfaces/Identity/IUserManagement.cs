using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserManagement
{
    /// <summary>
    /// Indicates whether the implementation supports direct user creation
    /// </summary>
    bool SupportsUserCreation { get; }
    
    /// <summary>
    /// Gets all users
    /// </summary>
    Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new user
    /// </summary>
    Task<bool> CreateUserAsync(UserDto user, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<bool> UpdateUserAsync(string userId, UserDto user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a user
    /// </summary>
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resets a user's password
    /// </summary>
    Task<bool> ResetPasswordAsync(string userId, CredentialDto credential, CancellationToken cancellationToken = default);
}