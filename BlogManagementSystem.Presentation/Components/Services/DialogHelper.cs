using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlogManagementSystem.Presentation.Components.Services
{
    public class DialogHelper(IDialogService dialogService)
    {
        public async Task<bool> ShowConfirmationAsync<TDialog>(string title, string message, string yesText = "Yes", string noText = "No") where TDialog : ComponentBase
        {
            var parameters = new DialogParameters
            {
                { "ContentText", message },
                { "ButtonText", yesText },
                { "CancelText", noText }
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var dialog = await dialogService.ShowAsync<TDialog>(title, parameters, options);
            var result = await dialog.Result;
            
            return !result.Canceled;
        }

        public async Task<TResult> ShowFormDialogAsync<TDialog, TResult>(string title, DialogParameters parameters = null) where TDialog : ComponentBase
        {
            parameters ??= new DialogParameters();
            
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            var dialog = await dialogService.ShowAsync<TDialog>(title, parameters, options);
            var result = await dialog.Result;
            
            return result.Canceled ? default : (TResult)result.Data;
        }
    }
}