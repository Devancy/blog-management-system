using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface IAppSettingRepository
{
    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the value of a setting as the specified type, or returns the default value if not found.
    /// </summary>
    Task<T> GetValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all settings.
    /// </summary>
    Task<List<AppSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets a setting value (creates if it doesn't exist or updates if it does).
    /// </summary>
    Task<AppSetting> SetValueAsync(string key, string value, string? description = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a setting by its key.
    /// </summary>
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
}