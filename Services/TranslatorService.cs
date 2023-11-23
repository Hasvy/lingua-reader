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

        public async Task<HttpResponseMessage> SetLanguages(string bookLang, string targetLang)
        {
            string apiUri = $"api/Translator/Set-languages?bookLang={bookLang}&targetLang={targetLang}";
            var response = await _httpClient.PostAsync(apiUri, null);
            return response;
        }

        public async Task<TranslatorWordResponse?> GetWordTranslation(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                TranslatorWordResponse? translation = await _httpClient.GetFromJsonAsync<TranslatorWordResponse?>($"api/Translator/TranslateWord?word={word}");
                if (translation is not null)
                {
                    return translation;
                }
            }

            return null;
        }

        //public async Task<Stream?> GetWordAudio(string word)
        //{
        //    if (!string.IsNullOrWhiteSpace(word))
        //    {
        //        Stream audio = await _httpClient.GetStreamAsync($"api/Translator/Speak?word={word}");
        //        if(audio is not null)
        //        {
        //            return audio;
        //        }
        //    }

        //    return null;
        //}

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
