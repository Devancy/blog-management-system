using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface ILocalGroupRepository
{
    Task<LocalGroup> CreateAsync(LocalGroup group, CancellationToken cancellationToken);
    Task<LocalGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LocalGroup?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<LocalGroup?> GetByPathAsync(string path, CancellationToken cancellationToken);
    Task<List<LocalGroup>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<LocalGroup>> GetRootGroupsAsync(CancellationToken cancellationToken);
    Task<List<LocalGroup>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<List<string>> GetUserIdsInGroupAsync(Guid groupId, CancellationToken cancellationToken);
    Task UpdateAsync(LocalGroup group, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task AddUserToGroupAsync(string userId, Guid groupId, CancellationToken cancellationToken);
    Task RemoveUserFromGroupAsync(string userId, Guid groupId, CancellationToken cancellationToken);
} 