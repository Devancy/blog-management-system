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

public class LocalUserIdentityRepository : ILocalUserIdentityRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public LocalUserIdentityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<LocalUserIdentity?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }
    
    public async Task<LocalUserIdentity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
    
    public async Task<LocalUserIdentity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
    
    public async Task<List<LocalUserIdentity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<LocalUserIdentity> CreateAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        userIdentity.CreatedAt = DateTime.UtcNow;
        userIdentity.LastLoginAt = DateTime.UtcNow;
        
        await _dbContext.LocalUserIdentities.AddAsync(userIdentity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return userIdentity;
    }
    
    public async Task<LocalUserIdentity> UpdateAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        userIdentity.UpdatedAt = DateTime.UtcNow;
        
        _dbContext.LocalUserIdentities.Update(userIdentity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return userIdentity;
    }
    
    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdentity = await _dbContext.LocalUserIdentities
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
            
        if (userIdentity == null)
            return false;
            
        _dbContext.LocalUserIdentities.Remove(userIdentity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .AnyAsync(u => u.UserId == userId, cancellationToken);
    }
    
    public async Task<LocalUserIdentity> UpsertAsync(LocalUserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.LocalUserIdentities
            .FirstOrDefaultAsync(u => u.UserId == userIdentity.UserId, cancellationToken);
            
        if (existing == null)
        {
            return await CreateAsync(userIdentity, cancellationToken);
        }
        
        // Update properties
        existing.Username = userIdentity.Username;
        existing.Email = userIdentity.Email;
        existing.FirstName = userIdentity.FirstName;
        existing.LastName = userIdentity.LastName;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.LastLoginAt = DateTime.UtcNow;
        
        return await UpdateAsync(existing, cancellationToken);
    }
    
    public async Task<List<LocalUserIdentity>> GetUsersWithRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<LocalUserIdentity>> GetUsersInGroupAsync(string groupPath, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalUserIdentities
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
            .Where(u => u.UserGroups.Any(ug => ug.Group.Path == groupPath))
            .ToListAsync(cancellationToken);
    }
} 