using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class WordsService
    {
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;
        public WordsService(HttpClient httpClient, NotificationService notificationService)
        {
            _httpClient = httpClient;
            _notificationService = notificationService;
        }

        public async Task<bool> SaveWord(WordWithTranslations wordWithTranslations)
        {
            var result = await _httpClient.PostAsJsonAsync("api/words/SaveWord", wordWithTranslations.Id);
            if (result.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Word has been saved");
                return true;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "An error occurred");
                return false;
            }
        }

        public async Task<bool> DeleteWord(WordWithTranslations wordWithTranslations)
        {
            var result = await _httpClient.DeleteAsync($"api/words/DeleteWord/{wordWithTranslations.Id}");
            if (result.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Info, "Word has been deleted");
                return true;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "An error occurred");
                return false;
            }
        }

        public async Task<List<WordWithTranslations>?> GetUsersWords()
        {
            var result = await _httpClient.GetFromJsonAsync<List<WordWithTranslations>>("api/words/GetUsersWords");
            if (result is not null)
            {
                return result;
            }
            return null;
        }
    }
}
