using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Dto;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using Radzen.Blazor;
using Services;
using System.Text;

namespace BlazorApp.Pages.MainPages
{
    public partial class Learning : ComponentBase, IDisposable
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        private List<WordWithTranslations> _allUserWords = new List<WordWithTranslations>();
        private List<WordToLearn> _wordsToLearn = new List<WordToLearn>();
        private bool _showStatistics = false;
        private bool _showPractice = false;
        private bool _isLoading;
        private bool _isSaving = false;
        private bool _isDeleting = false;
        private IList<WordWithTranslations>? _selectedWords;
        private IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 30 };
        private RadzenDataGrid<WordWithTranslations> grid;
        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            _isLoading = true;
            await GetAllUserWords();
            await base.OnInitializedAsync();
            _isLoading = false;
        }

        public async Task StartPractice()
        {
            if (_allUserWords.Count < 10)
            {
                await DialogService.Alert("You don't have enough words to learn. You have to add at least 10 words in specified language to learn.", "Notification", new AlertOptions()
                {
                    OkButtonText = "Go to Reading page"
                }
                );
                NavigationManager.NavigateTo(Routes.Reading);
            }
            else
            {
                Random random = new Random();
                _wordsToLearn = await WordsService.GetWordsToLearn();
                foreach (var wordToLearn in _wordsToLearn)
                {
                    wordToLearn.VariantsToAnswer.Add(new VariantToAnswer(wordToLearn.WordWithTranslations.Translations.First(), true));
                    foreach (var wrongVariant in wordToLearn.WrongVariants)
                    {
                        int index = random.Next(wordToLearn.VariantsToAnswer.Count() + 1);
                        wordToLearn.VariantsToAnswer.Insert(index, new VariantToAnswer(wrongVariant));
                    }
                }
                _showStatistics = false;
                _showPractice = true;
            }
        }

        public async Task GetAllUserWords()
        {
            _allUserWords = await WordsService.GetAllUsersWords();
        }

        public void EndPractice()
        {
            _showPractice = false;
            _showStatistics = true;
        }

        private async Task AddWord(WordWithTranslations word)
        {
            _isSaving = true;
            bool result = await WordsService.SaveWord(word.Id);
            if (result is true)
            {
                word.IsWordSaved = true;
                _allUserWords.Add(word);
            }
            _isSaving = false;
        }

        public async Task DeleteWord(WordWithTranslations word)
        {
            _isDeleting = true;
            var result = await WordsService.DeleteWord(word.Id);
            if (result is true)
            {
                word.IsWordSaved = false;
                _allUserWords.RemoveAll(w => w.Id == word.Id);
            }
            _isDeleting = false;
        }

        public async Task DeleteSelectedWords()
        {
            if (_selectedWords is not null && _selectedWords.Any())
            {
                _isDeleting = true;
                var result = await WordsService.DeleteWords(_selectedWords);
                if (result is true && _allUserWords is not null)
                    _allUserWords.RemoveAll(_selectedWords.Contains);
                await grid.RefreshDataAsync();
                _selectedWords = null;
                _isDeleting = false;
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "You didn't select any word");
            }
        }

        public async Task CsvExport()
        {
            List<WordWithTranslations> wordsToExport;
            if (_selectedWords is not null && _selectedWords.Any())
                wordsToExport = _selectedWords.ToList();
            else
                wordsToExport = _allUserWords;

            StringBuilder csvContent = new StringBuilder();
            foreach (var word in wordsToExport)
            {
                StringBuilder translations = new StringBuilder();
                foreach (var translation in word.Translations)
                {
                    translations.Append(translation.DisplayTarget).Append("; ");
                }

                csvContent.AppendLine($"{word.DisplaySource},{translations}");
            }

            byte[] data = Encoding.UTF8.GetBytes(csvContent.ToString());
            var memoryStream = new MemoryStream(data);
            var fileName = "my-words.csv";
            using var streamRef = new DotNetStreamReference(memoryStream);
            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }

        public async Task ShowWordsList()
        {
            await grid.RefreshDataAsync();
            _showStatistics = false;
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
