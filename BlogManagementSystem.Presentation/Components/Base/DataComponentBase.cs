using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlogManagementSystem.Presentation.Components.Base
{
    public abstract class DataComponentBase : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; } = null!;
        
        protected bool IsLoading { get; set; } = false;
        protected string SearchString { get; set; } = string.Empty;
        
        protected async Task ExecuteWithLoadingAsync(Func<Task> action, string successMessage = null)
        {
            try
            {
                IsLoading = true;
                await action();
                
                if (!string.IsNullOrEmpty(successMessage))
                {
                    Snackbar.Add(successMessage, Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
        
        protected async Task<T> ExecuteWithLoadingAsync<T>(Func<Task<T>> action, string successMessage = null)
        {
            try
            {
                IsLoading = true;
                var result = await action();
                
                if (!string.IsNullOrEmpty(successMessage))
                {
                    Snackbar.Add(successMessage, Severity.Success);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
                return default;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}