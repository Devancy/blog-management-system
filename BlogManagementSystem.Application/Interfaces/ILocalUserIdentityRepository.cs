using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface ILocalUserIdentityRepository
{
    Task<LocalUserIdentity?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<LocalUserIdentity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<LocalUserIdentity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<LocalUserIdentity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LocalUserIdentity> CreateAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default);
    Task<LocalUserIdentity> UpdateAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<LocalUserIdentity> UpsertAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default);
    Task<List<LocalUserIdentity>> GetUsersWithRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<List<LocalUserIdentity>> GetUsersInGroupAsync(string groupPath, CancellationToken cancellationToken = default);
} 