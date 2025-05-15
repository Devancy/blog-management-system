namespace BlogManagementSystem.Application.Interfaces;

public interface IAppSettingService
{
	Task<T> GetSettingAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
	Task SetSettingAsync<T>(string key, T value, string? description = null, CancellationToken cancellationToken = default);
}