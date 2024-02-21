using Microsoft.AspNetCore.Components;
using Services;

namespace BlazorApp.Pages.MainPages
{
    public partial class Learning : ComponentBase, IDisposable
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
