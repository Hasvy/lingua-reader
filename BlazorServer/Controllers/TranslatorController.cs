using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Objects;
using Objects.Entities.Translator;
using System.Text;

namespace BlazorServer.Controllers
{

    public class TranslatorController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly DictionaryDbContext _dictionaryDbContext;
        private static readonly string _key = "e3e05bc92ae54af883a388e21cedb21f";
        private static readonly string _endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string _location = "westeurope";
        private static readonly string _intermediateLang = ConstLanguages.English;          //Because microsoft translator supports translation only with english source or target language
        private static string _bookLang = ConstLanguages.English;       //Default value
        private static string _targetLang = ConstLanguages.Czech;       //Default value
        private int existedWordId = 0;

        //TODO Mb for test and filling db with data use recurse translation with backtranslations
        public TranslatorController(DictionaryDbContext dictionaryDbContext)
        {
            _httpClient = new HttpClient();
            _dictionaryDbContext = dictionaryDbContext;
        }

        //TODO return result of action
        [HttpPost]
        [Route("api/Translator/Set-languages")]
        public void SetLanguages([FromQuery] string bookLang, [FromQuery] string targetLang)        
        {
            if (bookLang is not ConstLanguages.Undefined && bookLang is not null)
            {
                _bookLang = bookLang;
                _targetLang = targetLang;
            }
        }

        [HttpGet]
        [Route("api/Translator/TranslateWord")]
        public async Task<ActionResult<TranslatorWordResponse?>> TranslateWord(string word)
        {
            existedWordId = 0;
            var translatorWordResp = await GetTranslationFromDb(word);
            if (translatorWordResp is not null)
            {
                return translatorWordResp;
            }

            if (_bookLang is not ConstLanguages.English && _targetLang is not ConstLanguages.English)
            {
                var englishWordResponse = await GetEngTranslation(word);
                if (englishWordResponse is not null)
                {
                    var firstTranslation = englishWordResponse.Translations.FirstOrDefault();
                    if (firstTranslation is not null)
                    {
                        word = firstTranslation.DisplayTarget;
                    }
                    else
                    {
                        Console.WriteLine($"Translation of {englishWordResponse.DisplaySource} is not found.");
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
                if (result is not null)
                {
                    var firstResult = result.FirstOrDefault();
                    if (firstResult is not null)
                    {
                        firstResult.DisplaySource = word.ToLower();
                        firstResult.Language = _bookLang;
                        await PostTranslationToDb(firstResult);
                        Console.WriteLine("Word added to Db");
                        return Ok(firstResult);
                    }
                }
                return Ok(null);      //TODO if result is null or translations is empty => word did not found
            }
            else
            {
                return BadRequest("Error: " + response.StatusCode);
            }
        }

        //[HttpGet]
        //[Route("api/Translator/Speak")]
        //public async Task<ActionResult<Stream>> SpeakWord(string word)
        //{
        //    SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        //    Stream audioStream = null;
        //    speechSynthesizer.SetOutputToWaveStream(audioStream);
        //    speechSynthesizer.Speak(word);
        //    return Ok(audioStream);
        //}

        private async Task<TranslatorWordResponse?> GetTranslationFromDb(string word)       //TODO divide word saving to different language database and search in target language db
        {
            var translatorWordResp = _dictionaryDbContext.Words.SingleOrDefault(w => w.DisplaySource == word && w.Language == _bookLang);
            if (translatorWordResp is not null)
            {
                existedWordId = translatorWordResp.Id;      //This will used in PostTranslationToDb to decide if word exist or not
                IList<WordTranslation> wordTranslations = _dictionaryDbContext.Translations.Where(t => t.WordId == translatorWordResp.Id && t.Language == _targetLang).ToList();
                if (!wordTranslations.Any())
                {
                    return null;
                }
                translatorWordResp.Translations = wordTranslations;
                return translatorWordResp;
            }
            else
            {
                //Word not found logic
                return null;
            }
        }

        private async Task<IActionResult> PostTranslationToDb(TranslatorWordResponse response)
        {
            //TODO dont put word in db if it does not contain any translation
            //TODO put word in db in right language, not in english (before try to change translation logic)
            if (ModelState.IsValid)
            {
                //bool isWordExist = _dictionaryDbContext.Words.Contains();
                foreach (var translation in response.Translations)
                {
                    translation.Language = _targetLang;
                    if (existedWordId != 0)         //If word already exist in Db, FK of translations changes to it, so word is not duplicated
                    {
                        translation.WordId = existedWordId;
                        _dictionaryDbContext.Translations.Add(translation);
                    }
                }
                if (existedWordId == 0)
                {
                    _dictionaryDbContext.Words.Add(response);       //TODO Fix saving deutsch words in english, so them wont found because it searching german word, in db words in eng
                }
                int updNumber = await _dictionaryDbContext.SaveChangesAsync();
                if (updNumber > 0)
                {
                    return Ok();
                }
            }

            return BadRequest();
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
