using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using Services;

namespace BlazorApp.Pages.MainPages
{
    public partial class Learning : ComponentBase, IDisposable
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        private List<WordWithTranslations> usersWords = new List<WordWithTranslations>();
        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            usersWords = await WordsService.GetUsersWords();
            await base.OnInitializedAsync();
        }



        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
