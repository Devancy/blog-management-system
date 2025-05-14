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

public class LocalRoleRepository : ILocalRoleRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public LocalRoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<LocalRole> CreateAsync(LocalRole role, CancellationToken cancellationToken)
    {
        await _dbContext.LocalRoles.AddAsync(role, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role;
    }
    
    public async Task<LocalRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalRoles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
    
    public async Task<LocalRole?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalRoles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
    
    public async Task<List<LocalRole>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.LocalRoles
            .Include(r => r.UserRoles)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<LocalRole>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalRoles
            .Include(r => r.UserRoles)
            .Where(r => r.UserRoles.Any(ur => ur.UserId == userId))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<LocalRole>> GetByGroupPathAsync(string groupPath, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalGroupRoles
            .Include(gr => gr.Role)
            .Include(gr => gr.Group)
            .Where(gr => gr.Group.Path == groupPath)
            .Select(gr => gr.Role)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<string>> GetUserIdsInRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _dbContext.LocalUserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(LocalRole role, CancellationToken cancellationToken)
    {
        _dbContext.LocalRoles.Update(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // Remove all user role associations
        var userRoles = await _dbContext.LocalUserRoles
            .Where(ur => ur.RoleId == id)
            .ToListAsync(cancellationToken);
            
        _dbContext.LocalUserRoles.RemoveRange(userRoles);
        
        // Remove all group role associations
        var groupRoles = await _dbContext.LocalGroupRoles
            .Where(gr => gr.RoleId == id)
            .ToListAsync(cancellationToken);
            
        _dbContext.LocalGroupRoles.RemoveRange(groupRoles);
        
        // Remove the role
        var role = await _dbContext.LocalRoles.FindAsync(new object[] { id }, cancellationToken);
        if (role != null)
        {
            _dbContext.LocalRoles.Remove(role);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddUserToRoleAsync(string userId, Guid roleId, CancellationToken cancellationToken)
    {
        // Check if the assignment already exists
        var exists = await _dbContext.LocalUserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            
        if (!exists)
        {
            var userRole = new LocalUserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _dbContext.LocalUserRoles.AddAsync(userRole, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task RemoveUserFromRoleAsync(string userId, Guid roleId, CancellationToken cancellationToken)
    {
        var userRole = await _dbContext.LocalUserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            
        if (userRole != null)
        {
            _dbContext.LocalUserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task AssignRoleToGroupAsync(Guid roleId, Guid groupId, CancellationToken cancellationToken)
    {
        // Check if the assignment already exists
        var exists = await _dbContext.LocalGroupRoles
            .AnyAsync(gr => gr.GroupId == groupId && gr.RoleId == roleId, cancellationToken);
            
        if (!exists)
        {
            var groupRole = new LocalGroupRole
            {
                GroupId = groupId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _dbContext.LocalGroupRoles.AddAsync(groupRole, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task RemoveRoleFromGroupAsync(Guid roleId, Guid groupId, CancellationToken cancellationToken)
    {
        var groupRole = await _dbContext.LocalGroupRoles
            .FirstOrDefaultAsync(gr => gr.GroupId == groupId && gr.RoleId == roleId, cancellationToken);
            
        if (groupRole != null)
        {
            _dbContext.LocalGroupRoles.Remove(groupRole);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
} 