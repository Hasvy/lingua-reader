using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Objects.Entities.Translator;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Services
{
    public class TranslatorService
    {
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;

        public TranslatorService(HttpClient httpClient, NotificationService notificationService)
        {
            _httpClient = httpClient;
            _notificationService = notificationService;
        }

        //public async Task<HttpResponseMessage> SetLanguage(string bookLang)
        //{
        //    string apiUri = $"api/Translator/Set-languages?bookLang={bookLang}";
        //    var response = await _httpClient.PostAsync(apiUri, null);
        //    return response;
        //}

        public async Task<WordWithTranslations?> GetWordTranslation(string word, string sourceLang)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                WordWithTranslations? wordWithTranslations = await _httpClient.GetFromJsonAsync<WordWithTranslations?>($"api/Translator/TranslateWord?word={word}&sourceLang={sourceLang}");
                if (wordWithTranslations is not null && wordWithTranslations.Translations.Any())
                {
                    return wordWithTranslations;
                }
            }

            return null;
        }

        public async Task<WordWithTranslations?> UpdateWord(WordWithTranslations word)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Translator/UpdateWord", word);
            if (response.IsSuccessStatusCode)
            {
                WordWithTranslations wordWithTranslations;
                var updatedWord = await response.Content.ReadFromJsonAsync<WordWithTranslations>();
                if (updatedWord != null)
                {
                    if (IsWordTranslationsEqual(word, updatedWord))
                    {
                        wordWithTranslations = word;
                        _notificationService.Notify(NotificationSeverity.Info, "Translations are actual", "Translator returns the same");
                    }
                    else
                    {
                        wordWithTranslations = updatedWord;
                        _notificationService.Notify(NotificationSeverity.Success, "Word has been updated");
                    }
                    return wordWithTranslations;
                }
                return null;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "An error occurred");
                return null;
            }
        }

        private bool IsWordTranslationsEqual(WordWithTranslations word1, WordWithTranslations word2)
        {
            var oldTranslations = word1.Translations.Select(t => t.DisplayTarget);
            var newTranslations = word2.Translations.Select(t => t.DisplayTarget);
            if (oldTranslations.SequenceEqual(newTranslations))
                return true;
            else
                return false;
        }

        public async Task<string?> GetTextTranslation()
        {
            string? word = null;
            TranslatorTextResponse? translation = await _httpClient.GetFromJsonAsync<TranslatorTextResponse>("api/Translator/TranslateText");
            if (translation is not null && translation.translations.First() is not null)
            {
                word = translation.translations.First().text;
            }

            return word;
        }
    }
}
