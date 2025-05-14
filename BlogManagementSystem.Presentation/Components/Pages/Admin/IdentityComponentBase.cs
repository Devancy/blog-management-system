using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlogManagementSystem.Presentation.Components.Pages.Admin;

public abstract class IdentityComponentBase : ComponentBase
{
    [Inject] protected IIdentityManagerFactory IdentityManagerFactory { get; set; } = null!;
    [Inject] protected IdentityConfig Config { get; set; } = null!;
    [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
    
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
            var val = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "identityMode");
            return string.IsNullOrEmpty(val) ? IdentityMode.Keycloak : Enum.Parse<IdentityMode>(val);
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
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "identityMode", mode);
        }
        catch
        {
            // Ignore errors storing the preference
        }
    }
}