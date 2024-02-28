using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using Services;
using System.Diagnostics;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BlazorApp.Pages.MainPages
{
    public partial class Learning : ComponentBase, IDisposable
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        //private List<WordWithTranslations>? usersWords = new List<WordWithTranslations>();
        private List<WordToLearn>? _wordsToLearn = new List<WordToLearn>();
        private WordsToLearnDto? _wordsToLearnDto = null;
        private bool _isLoading;
        private int _actualCardNumber = 0;
        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            _isLoading = true;
            _wordsToLearnDto = await WordsService.GetWordsToLearn();
            if (_wordsToLearnDto != null)
            {
                if (_wordsToLearnDto.WordsCount < 10)
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
                    var ints = new List<int>();
                    _wordsToLearn = _wordsToLearnDto.WordsToLearn;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    foreach (var wordToLearn in _wordsToLearn)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            wordToLearn.VariantsToAnswer.Add(new VariantToAnswer(wordToLearn.WordWithTranslations.Translations.First(), true));
                            foreach (var wrongVariant in wordToLearn.WrongVariants)
                            {
                                int index = random.Next(wordToLearn.VariantsToAnswer.Count() + 1);                //Add tests to a diplom
                                wordToLearn.VariantsToAnswer.Insert(index, new VariantToAnswer(wrongVariant));
                                //wordToLearn.VariantsToAnswer.Insert(0, new VariantToAnswer(wrongVariant));
                            }
                            //wordToLearn.VariantsToAnswer = GenerateRandomLoop(wordToLearn.VariantsToAnswer);
                            if (i != 99)
                            {
                                ints.Add(wordToLearn.VariantsToAnswer.FindIndex(v => v.isRight) + 1);
                                wordToLearn.VariantsToAnswer.Clear();
                            }
                        }
                    }
                    stopwatch.Stop();
                    for (int i = 1; i <= 4; i++)
                    {
                        Console.WriteLine($"{i}: " + ints.Count(num => num == i));
                    }
                    Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms" + "\n");

                    await base.OnInitializedAsync();
                }
            }
            _isLoading = false;
        }

        public async Task ChangeCard()
        {
            if (_actualCardNumber < _wordsToLearn.Count() - 1)
            {
                _actualCardNumber++;
            }
            else
            {
                //TODO show statistics of the round
            }
        }

        public List<VariantToAnswer> GenerateRandomLoop(List<VariantToAnswer> listToShuffle)
        {
            Random random = new Random();
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = random.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
