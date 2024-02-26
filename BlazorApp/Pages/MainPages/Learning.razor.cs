using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using Services;

namespace BlazorApp.Pages.MainPages
{
    public partial class Learning : ComponentBase, IDisposable
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<WordWithTranslations>? usersWords = new List<WordWithTranslations>();
        private int userWordsCount = 0;
        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            userWordsCount = await WordsService.GetWordsCount();
            if (userWordsCount < 10)
            {
                await DialogService.Alert("You don't have enough words to learn. You have to add at least 10 words in specified language to learn.", "Notification", new AlertOptions()
                    {
                        OkButtonText = "Go to Reading page"
                    }
                );
                NavigationManager.NavigateTo(Routes.Reading);
            }
            usersWords = await WordsService.GetAllUsersWords();
            await base.OnInitializedAsync();
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
