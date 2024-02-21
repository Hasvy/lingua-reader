using Microsoft.AspNetCore.Components;
using Services;

namespace BlazorApp.Pages
{
    public partial class Reading : ComponentBase, IDisposable
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            await base.OnInitializedAsync();
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
