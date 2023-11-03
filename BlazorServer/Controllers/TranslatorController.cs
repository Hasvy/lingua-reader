using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Objects.Entities.Translator;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using Objects;
using UglyToad.PdfPig.Content;
using System.Speech.Synthesis;
using Objects.Entities.Translator.Words;

namespace BlazorServer.Controllers
{

    public class TranslatorController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static readonly string _key = "e3e05bc92ae54af883a388e21cedb21f";
        private static readonly string _endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string _location = "westeurope";
        private static readonly string _intermediateLang = ConstLanguages.English;          //Because microsoft translator supports translation only with english source or target language
        private static string _bookLang = ConstLanguages.English;       //Default value
        private static string _targetLang = ConstLanguages.Czech;       //Default value

        public TranslatorController()
        {
            _httpClient = new HttpClient();
        }

        [HttpPost]
        [Route("api/Translator/Set-book-language")]
        public void SetBookLang([FromBody] string bookLang)        //TODO return result of action
        {
            if (bookLang is not ConstLanguages.Undefined || bookLang is not null)
            {
                _bookLang = bookLang;
            }
            else
            {
                Console.WriteLine("Please specify language of the book");
            }
        }

        [HttpPost]
        [Route("api/Translator/Set-target-language")]
        public void SetTargetLang([FromBody] string targetLang)
        {
            if (targetLang is not null)
            {
                _targetLang = targetLang;
            }
            else
            {
                Console.WriteLine("Please specify your language in settings");
            }
        }

        [HttpGet]
        [Route("api/Translator/TranslateWord")]
        public async Task<ActionResult<TranslatorWordResponse?>> TranslateWord(string word)
        {
            if (_bookLang is not ConstLanguages.English && _targetLang is not ConstLanguages.English)
            {
                var englishWordResponse = await GetEngTranslation(word);
                if (englishWordResponse is not null)
                {
                    var firstTranslation = englishWordResponse.translations.FirstOrDefault();
                    if (firstTranslation is not null)
                    {
                        word = firstTranslation.displayTarget;
                    }
                    else
                    {
                        Console.WriteLine($"Translation of {englishWordResponse.displaySource} is not found.");
                    }
                }
                //else
                //{
                //    throw new Exception("Error during translation to intrmediateLang.");
                //}
            }

            HttpResponseMessage response = await ConstructAndSendQuery(word, _intermediateLang, _targetLang);

            if (response.IsSuccessStatusCode)
            {
                List<TranslatorWordResponse>? result = await response.Content.ReadFromJsonAsync<List<TranslatorWordResponse>>();
                //SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
                //speechSynthesizer.Speak(word);      //TODO return stream or file and play it on client
                return Ok(result.FirstOrDefault());      //TODO if result is null or translations is empty => word did not found
            }
            else
            {
                return BadRequest("Error: " + response.StatusCode);
            }

        }

        private async Task<TranslatorWordResponse?> GetEngTranslation(string word)
        {
            HttpResponseMessage response = await ConstructAndSendQuery(word, _bookLang, _intermediateLang);
            if (response.IsSuccessStatusCode)
            {
                List<TranslatorWordResponse>? result = await response.Content.ReadFromJsonAsync<List<TranslatorWordResponse>>();
                if (result is not null)
                {
                    TranslatorWordResponse? wordResponse = result.FirstOrDefault();
                    if (wordResponse is not null)
                    {
                        return wordResponse;
                    }
                }
            }
            return null;
        }

        private async Task<HttpResponseMessage> ConstructAndSendQuery(string word, string sourceLang, string targetLang)
        {
            string route = $"dictionary/lookup?api-version=3.0&from={sourceLang}&to={targetLang}";
            object[] body = new object[] { new { Text = word } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

                return await _httpClient.SendAsync(request).ConfigureAwait(false);
            }
        }


        [HttpGet]
        [Route("api/Translator/TranslateText")]
        public async Task<ActionResult<TranslatorTextResponse>> TranslateText()
        {
            try
            {
                string route = "/translate?api-version=3.0&from=en&to=cs";
                string textToTranslate = "Hello";
                object[] body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(_endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

                    HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        List<TranslatorTextResponse?>? result = await response.Content.ReadFromJsonAsync<List<TranslatorTextResponse?>?>();
                        return Ok(result.FirstOrDefault());
                    }
                    else
                    {
                        return BadRequest("Error: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: " + ex.Message);
            }
        }
    }
}
