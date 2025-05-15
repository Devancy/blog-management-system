using BlogManagementSystem.Application.Common;
using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Services;
using Microsoft.AspNetCore.Components;

namespace BlogManagementSystem.Presentation.Components.Pages.Admin;

public abstract class IdentityComponentBase : ComponentBase
{
    [Inject] protected IIdentityManagerFactory IdentityManagerFactory { get; set; } = null!;
    [Inject] protected IdentityConfig Config { get; set; } = null!;
    [Inject] protected AppSettingService AppSettingService { get; set; } = null!;
    
    protected IIdentityManager IdentityManager => IdentityManagerFactory.CurrentManager;
    protected IdentityMode CurrentMode => Config.UseKeycloakAsIdpProxy ? IdentityMode.Proxy : IdentityMode.Keycloak;
    
    protected override async Task OnInitializedAsync()
    {
        // Try to load the preferred mode from local storage
        var storedMode = await GetStoredModeAsync();
        if (storedMode != CurrentMode)
        {
            await IdentityManagerFactory.Initialize(storedMode);
            await OnModeChangedAsync();
        }
        
        await base.OnInitializedAsync();
    }
    
    protected async Task ChangeIdentityModeAsync(IdentityMode mode)
    {
        if (mode != CurrentMode)
        {
            await IdentityManagerFactory.Initialize(mode);
            await StorePreferredModeAsync(mode);
            await OnModeChangedAsync();
            StateHasChanged();
        }
    }
    
    protected virtual Task OnModeChangedAsync()
    {
        // Override in derived components to refresh data when mode changes
        return Task.CompletedTask;
    }
    
    private async Task<IdentityMode> GetStoredModeAsync()
    {
        try
        {
            var isProxyMode = await AppSettingService.GetSettingAsync(Constants.Identity.UseKeycloakAsIdpProxyKey, false);
            return isProxyMode ? IdentityMode.Proxy : IdentityMode.Keycloak;
        }
        catch
        {
            return IdentityMode.Keycloak;
        }
    }
    
    private async Task StorePreferredModeAsync(IdentityMode mode)
    {
        try
        {
            await AppSettingService.SetSettingAsync(Constants.Identity.UseKeycloakAsIdpProxyKey, mode == IdentityMode.Proxy);
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException("Failed to store preferred identity mode", ex);
        }
    }
}