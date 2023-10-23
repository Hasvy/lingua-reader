using Objects.Entities.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Services
{
    public class TranslatorService
    {
        private readonly HttpClient _httpClient;

        public TranslatorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TranslatorWordResponse?> GetWordTranslation(string word)
        {
            TranslatorWordResponse? translation = await _httpClient.GetFromJsonAsync<TranslatorWordResponse?>($"api/Proxy/TranslateWord/{word}");
            if (translation is not null)
            {
                return translation;
            }

            return null;
        }

        public async Task<string?> GetTextTranslation()
        {
            string? word = null;
            TranslatorTextResponse? translation = await _httpClient.GetFromJsonAsync<TranslatorTextResponse>("api/Proxy/TranslateText");
            if (translation is not null && translation.translations.First() is not null)
            {
                word = translation.translations.First().text;
            }

            return word;
        }
    }
}
