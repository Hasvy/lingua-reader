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

        public async Task<bool> SaveWord(TranslatorWordResponse translatorWordResponse)
        {
            var result = await _httpClient.PostAsJsonAsync("api/words/SaveWord", translatorWordResponse.Id);    //TODO dont allow to a user to send more than 1 query, save more than 1 word
            if (result.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Word is saved");
                return true;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Error is occured");
                return false;
            }
        }

        public async Task<bool> DeleteWord(TranslatorWordResponse translatorWordResponse)
        {
            var result = await _httpClient.DeleteAsync($"api/words/DeleteWord/{translatorWordResponse.Id}");
            if (result.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Word is deleted");
                return true;
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Error is occured");
                return false;
            }
        }

        public async Task<List<TranslatorWordResponse>> GetUsersWords()
        {
            var result = await _httpClient.GetFromJsonAsync<List<TranslatorWordResponse>>("api/words/GetUsersWords");
            if (result is not null)
            {
                return result;
            }
            return null;
        }
    }
}
