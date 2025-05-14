using System.Threading.Tasks;
using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlogManagementSystem.Presentation.Components.Pages.Admin.Identity;

public abstract class IdentityComponentBase : ComponentBase
{
    [Inject] protected IIdentityManagerFactory IdentityManagerFactory { get; set; } = null!;
    [Inject] protected IdentityConfig Config { get; set; } = null!;
    [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
    
    protected IIdentityManager IdentityManager => IdentityManagerFactory.CurrentManager;
    protected string CurrentMode => Config.UseKeycloakAsIdpProxy ? "proxy" : "keycloak";
    
    protected override async Task OnInitializedAsync()
    {
        // Try to load the preferred mode from local storage
        var storedMode = await GetStoredModeAsync();
        if (!string.IsNullOrEmpty(storedMode) && storedMode != CurrentMode)
        {
            await IdentityManagerFactory.Initialize(storedMode);
            await OnModeChangedAsync();
        }
        
        await base.OnInitializedAsync();
    }
    
    protected async Task ChangeIdentityModeAsync(string mode)
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
    
    private async Task<string> GetStoredModeAsync()
    {
        try
        {
            return await JsRuntime.InvokeAsync<string>("localStorage.getItem", "identityMode") ?? "keycloak";
        }
        catch
        {
            return "keycloak";
        }
    }
    
    private async Task StorePreferredModeAsync(string mode)
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