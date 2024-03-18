using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using System.Net.Http.Json;

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

        public async Task<bool> SaveWord(int id)
        {
            var result = await _httpClient.PostAsJsonAsync("api/words/SaveWord", id);
            return NotifyAndReturn(result.IsSuccessStatusCode, "Word has been saved");
        }

        public async Task<bool> DeleteWord(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/words/DeleteWord/{id}");
            return NotifyAndReturn(result.IsSuccessStatusCode, "Word has been deleted");
        }
        public async Task<bool> DeleteWords(IList<WordWithTranslations> wordsWithTranslations)
        {
            var wordsIds = wordsWithTranslations.Select(w => w.Id).ToList();
            var result = await _httpClient.PostAsJsonAsync("api/words/DeleteWords", wordsIds);
            return NotifyAndReturn(result.IsSuccessStatusCode, "Words have been deleted");
        }

        public async Task<List<WordToLearn>> GetWordsToLearn()
        {
            var result = await _httpClient.GetFromJsonAsync<List<WordToLearn>>("api/words/GetWordsToLearn");
            if (result is not null)
                return result;
            return new List<WordToLearn>();
        }

        public async Task<List<WordWithTranslations>> GetAllUsersWords()
        {
            var result = await _httpClient.GetFromJsonAsync<List<WordWithTranslations>>("api/words/GetAllUsersWords");
            if (result is not null)
                return result;
            return new List<WordWithTranslations>();
        }

        private bool NotifyAndReturn(bool StatusCode, string message)
        {
            if (StatusCode is true)
            {
                _notificationService.Notify(NotificationSeverity.Success, message);
                return true;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "An error occurred");
                return false;
            }
        }
    }
}