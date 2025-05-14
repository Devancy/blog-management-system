using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface ILocalRoleRepository
{
    Task<LocalRole> CreateAsync(LocalRole role, CancellationToken cancellationToken);
    Task<LocalRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LocalRole?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<LocalRole>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<LocalRole>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<List<LocalRole>> GetByGroupPathAsync(string groupPath, CancellationToken cancellationToken);
    Task<List<string>> GetUserIdsInRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task UpdateAsync(LocalRole role, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task AddUserToRoleAsync(string userId, Guid roleId, CancellationToken cancellationToken);
    Task RemoveUserFromRoleAsync(string userId, Guid roleId, CancellationToken cancellationToken);
    Task AssignRoleToGroupAsync(Guid roleId, Guid groupId, CancellationToken cancellationToken);
    Task RemoveRoleFromGroupAsync(Guid roleId, Guid groupId, CancellationToken cancellationToken);
} 