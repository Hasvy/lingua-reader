using BlazorApp.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace BlazorApp
{
    public partial class MainLayout
    {
        [Inject] DialogService DialogService { get; set; } = null!;

        private ErrorBoundary? errorBoundary;
        public async Task ShowInfoDialog()
        {
            await DialogService.OpenAsync<InfoDialog>($"Information",
               new Dictionary<string, object>(),
               new DialogOptions() { Width = "800px", Height = "512px", CloseDialogOnOverlayClick=true });
        }
    }
}
