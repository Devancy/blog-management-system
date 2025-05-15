using System.Text.Json;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;
using BlogManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Repositories;

public class AppSettingRepository(ApplicationDbContext dbContext) : IAppSettingRepository
{
    public async Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await dbContext.AppSettings
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }
    
    public async Task<T> GetValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        
        if (setting == null)
        {
            return defaultValue;
        }
        
        try
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)setting.Value;
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)bool.Parse(setting.Value);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(setting.Value);
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(object)double.Parse(setting.Value);
            }

            return JsonSerializer.Deserialize<T>(setting.Value) ?? defaultValue;
        }
        catch
        {
            // If conversion fails, return the default value
            return defaultValue;
        }
    }
    
    public async Task<List<AppSetting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AppSettings.ToListAsync(cancellationToken);
    }
    
    public async Task<AppSetting> SetValueAsync(string key, string value, string? description = null, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        
        if (setting == null)
        {
            setting = new AppSetting
            {
                Key = key,
                Value = value,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
            
            await dbContext.AppSettings.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.Value = value;
            
            if (description != null)
            {
                setting.Description = description;
            }
            
            setting.UpdatedAt = DateTime.UtcNow;
            dbContext.AppSettings.Update(setting);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return setting;
    }
    
    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        
        if (setting == null)
        {
            return false;
        }
        
        dbContext.AppSettings.Remove(setting);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}