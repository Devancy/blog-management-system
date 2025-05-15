using BlogManagementSystem.Application.Interfaces;

namespace BlogManagementSystem.Application.Services;

public class AppSettingService(IAppSettingRepository settingRepository) : IAppSettingService
{
    /// <summary>
    /// Retrieves a setting value from the database, or returns the default value if not found.
    /// </summary>
    public async Task<T> GetSettingAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        return await settingRepository.GetValueAsync(key, defaultValue, cancellationToken);
    }
    
    /// <summary>
    /// Sets a setting value in the database.
    /// </summary>
    public async Task SetSettingAsync<T>(string key, T value, string? description = null, CancellationToken cancellationToken = default)
    {
        string stringValue = value?.ToString() ?? string.Empty;
        await settingRepository.SetValueAsync(key, stringValue, description, cancellationToken);
    }
} 
