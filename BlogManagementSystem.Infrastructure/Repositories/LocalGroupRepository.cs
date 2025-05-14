using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;
using BlogManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Repositories;

public class LocalGroupRepository : ILocalGroupRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public LocalGroupRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<LocalGroup> CreateAsync(LocalGroup group, CancellationToken cancellationToken)
    {
        await _dbContext.LocalGroups.AddAsync(group, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return group;
    }
    
    public async Task<LocalGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.SubGroups)
            .Include(g => g.UserGroups)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
    
    public async Task<LocalGroup?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.SubGroups)
            .Include(g => g.UserGroups)
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
    }
    
    public async Task<LocalGroup?> GetByPathAsync(string path, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.SubGroups)
            .Include(g => g.UserGroups)
            .FirstOrDefaultAsync(g => g.Path == path, cancellationToken);
    }
    
    public async Task<List<LocalGroup>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.SubGroups).ThenInclude(sg => sg.SubGroups).ThenInclude(sg => sg.SubGroups)
            .Include(g => g.UserGroups)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<LocalGroup>> GetRootGroupsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.SubGroups).ThenInclude(sg => sg.SubGroups).ThenInclude(sg => sg.SubGroups)
            .Include(g => g.UserGroups)
            .Where(g => g.ParentGroupId == null)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<LocalGroup>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroups
            .Include(g => g.UserGroups)
            .Where(g => g.UserGroups.Any(ug => ug.UserId == userId))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<string>> GetUserIdsInGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalUserGroups
            .Where(ug => ug.GroupId == groupId)
            .Select(ug => ug.UserId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(LocalGroup group, CancellationToken cancellationToken)
    {
        _dbContext.LocalGroups.Update(group);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // Get all subgroups recursively
        var groupsToDelete = new List<LocalGroup>();
        var group = await _dbContext.LocalGroups
            .Include(g => g.SubGroups)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
            
        if (group == null)
            return;
            
        // Add this group and all subgroups to the delete list
        CollectGroupsToDelete(group, groupsToDelete);
        
        // Delete all user-group associations for these groups
        var groupIds = groupsToDelete.Select(g => g.Id).ToList();
        var userGroups = await _dbContext.LocalUserGroups
            .Where(ug => groupIds.Contains(ug.GroupId))
            .ToListAsync(cancellationToken);
            
        _dbContext.LocalUserGroups.RemoveRange(userGroups);
        
        // Delete groups from leaf to root
        foreach (var groupToDelete in groupsToDelete.OrderByDescending(g => g.Path.Count(c => c == '/')))
        {
            _dbContext.LocalGroups.Remove(groupToDelete);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddUserToGroupAsync(string userId, Guid groupId, CancellationToken cancellationToken)
    {
        // Check if the assignment already exists
        var exists = await _dbContext.LocalUserGroups
            .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId, cancellationToken);
            
        if (!exists)
        {
            var userGroup = new LocalUserGroup
            {
                UserId = userId,
                GroupId = groupId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _dbContext.LocalUserGroups.AddAsync(userGroup, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task RemoveUserFromGroupAsync(string userId, Guid groupId, CancellationToken cancellationToken)
    {
        var userGroup = await _dbContext.LocalUserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId, cancellationToken);
            
        if (userGroup != null)
        {
            _dbContext.LocalUserGroups.Remove(userGroup);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    private void CollectGroupsToDelete(LocalGroup group, List<LocalGroup> groupsToDelete)
    {
        groupsToDelete.Add(group);
        
        if (group.SubGroups != null)
        {
            foreach (var subGroup in group.SubGroups)
            {
                CollectGroupsToDelete(subGroup, groupsToDelete);
            }
        }
    }
} 