using AngleSharp.Io;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Objects.Constants;
using Objects.Entities;
using Objects.Entities.Translator;
using System.Net;
using System.Text;

namespace BlazorServer.Controllers
{

    public class TranslatorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly DictionaryDbContext _dictionaryDbContext;
        private static readonly string _endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string _location = "westeurope";
        private static readonly string _intermediateLang = ConstLanguages.English;          //Because microsoft translator supports translation only with english source or target language
        //private static string _bookLang = ConstLanguages.English;       //Default value
        //private static string _targetLang = ConstLanguages.Czech;       //Default value
        private static string _key = string.Empty;
        //private int existedWordId = 0;

        //TODO Mb for test and filling db with data use recurse translation with backtranslations
        public TranslatorController(DictionaryDbContext dictionaryDbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext appDbContext)
        {
            _httpClient = new HttpClient();
            _userManager = userManager;
            _appDbContext = appDbContext;
            _dictionaryDbContext = dictionaryDbContext;
            _configuration = configuration;
            var key = _configuration["Keys:TranslatorApiKey"];
            if (key is not null)
            {
                _key = key;
            }
        }

        //TODO return result of action
        //[HttpPost]
        //[Route("api/Translator/Set-languages")]
        //public async Task SetLanguages(string bookLang)
        //{
        //    if (bookLang is not ConstLanguages.Undefined && bookLang is not null)
        //    {
        //        _bookLang = bookLang;
        //        var user = await _userManager.GetUserAsync(User);
        //        _targetLang = user.NativeLanguage;
        //    }
        //}

        [HttpGet]
        [Route("api/Translator/TranslateWord")]
        public async Task<ActionResult<WordWithTranslations>?> TranslateWord(string word, string sourceLang)
        {
            int existedWordId = 0;
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();
            string bookLang = sourceLang;
            string targetLang = user.NativeLanguage;
            HttpResponseMessage response;

            var translatorWordResp = await GetTranslationFromDb(word, bookLang, targetLang, existedWordId);
            if (translatorWordResp is not null)
            {
                return Ok(translatorWordResp);
            }

            if (bookLang is not ConstLanguages.English && targetLang is not ConstLanguages.English)
            {
                response = await TranslateThroughEnglish(word, bookLang, targetLang);
            }
            else
            {
                response = await ConstructAndSendQuery(word, _intermediateLang, targetLang);
            }

            if (response.IsSuccessStatusCode)
            {
                List<WordWithTranslations>? result = await response.Content.ReadFromJsonAsync<List<WordWithTranslations>>();
                if (result is not null)
                {
                    var firstResult = result.FirstOrDefault();
                    if (firstResult is not null)
                    {
                        firstResult.DisplaySource = word.ToLower();
                        firstResult.Language = bookLang;
                        PostTranslationToDb(firstResult, existedWordId, targetLang);
                        return Ok(firstResult);
                    }
                }
                return Ok(new WordWithTranslations { DisplaySource = "" });         //Result is null or translations is empty => word did not found
            }
            else
            {
                return Ok(new WordWithTranslations { DisplaySource = "" });         //Result of translation of englishWord is not found => word did not found
            }
        }

        private async Task<HttpResponseMessage> TranslateThroughEnglish(string word, string bookLang, string targetLang)
        {
            var englishWordResponse = await GetEngTranslation(word, bookLang);
            if (englishWordResponse is not null)
            {
                var firstTranslation = englishWordResponse.Translations.FirstOrDefault();
                if (firstTranslation is not null)
                {
                    var englishWord = firstTranslation.DisplayTarget;
                    return await ConstructAndSendQuery(englishWord, _intermediateLang, targetLang);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);         //Translation of englishWord is not found => word did not found
                }
            }
            else
            {
                throw new Exception("Error during translation to intrmediateLang.");
            }
        }

        private async Task<WordWithTranslations?> GetTranslationFromDb(string word, string bookLang, string targetLang, int existedWordId)       //TODO divide word saving to different language database and search in target language db
        {
            var wordWithTranslations = _dictionaryDbContext.Words.SingleOrDefault(w => w.DisplaySource == word && w.Language == bookLang);     //RODO fix language in which word is saved
            if (wordWithTranslations is not null)
            {
                existedWordId = wordWithTranslations.Id;      //This will used in PostTranslationToDb to decide if word exist or not
                IList<WordTranslation> wordTranslations = _dictionaryDbContext.Translations.Where(t => t.WordId == wordWithTranslations.Id && t.Language == targetLang).ToList();
                if (!wordTranslations.Any())
                {
                    return null;
                }
                wordWithTranslations.Translations = wordTranslations;
                var user = await _userManager.GetUserAsync(User);
                wordWithTranslations.IsWordSaved = _appDbContext.SavedWords.Any(w => w.UserId.ToString() == user.Id && w.WordId == wordWithTranslations.Id);
                return wordWithTranslations;
            }
            else
            {
                //Word not found logic
                return null;
            }
        }

        private ActionResult<WordWithTranslations> PostTranslationToDb(WordWithTranslations wordWithTranslations, int existedWordId, string targetLang)
        {
            //TODO dont put word in db if it does not contain any translation
            //TODO put word in db in right language, not in english (before try to change translation logic)
            if (ModelState.IsValid)
            {
                //bool isWordExist = _dictionaryDbContext.Words.Contains();
                foreach (var translation in wordWithTranslations.Translations)
                {
                    translation.Language = targetLang;
                    if (existedWordId != 0)         //If word already exist in Db, FK of translations changes to it, so word is not duplicated in DB
                    {
                        translation.WordId = existedWordId;
                        _dictionaryDbContext.Translations.Add(translation);
                    }
                }
                if (existedWordId == 0)
                {
                    _dictionaryDbContext.Words.Add(wordWithTranslations);
                }
                else
                {
                    wordWithTranslations.Id = existedWordId;
                }
                int updNumber = _dictionaryDbContext.SaveChanges();
                if (updNumber > 0)
                {
                    return Ok(wordWithTranslations);
                }
            }

            return BadRequest();
        }

        private async Task<WordWithTranslations?> GetEngTranslation(string word, string bookLang)
        {
            HttpResponseMessage response = await ConstructAndSendQuery(word, bookLang, _intermediateLang);
            if (response.IsSuccessStatusCode)
            {
                List<WordWithTranslations>? result = await response.Content.ReadFromJsonAsync<List<WordWithTranslations>>();
                if (result is not null)
                {
                    WordWithTranslations? wordResponse = result.FirstOrDefault();
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
                request.Method = System.Net.Http.HttpMethod.Post;
                request.RequestUri = new Uri(_endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

                return await _httpClient.SendAsync(request).ConfigureAwait(false);
            }
        }

        [HttpPost]
        [Route("api/Translator/UpdateWord")]
        public async Task<ActionResult<WordWithTranslations>?> UpdateWord([FromBody] WordWithTranslations wordWithTranslations)
        {
            HttpResponseMessage response;
            if (wordWithTranslations.Language is not ConstLanguages.English && wordWithTranslations.Translations.First().Language is not ConstLanguages.English)
            {
                response = await TranslateThroughEnglish(wordWithTranslations.DisplaySource, wordWithTranslations.Language, wordWithTranslations.Translations.First().Language);
            }
            else
            {
                response = await ConstructAndSendQuery(wordWithTranslations.DisplaySource, _intermediateLang, wordWithTranslations.Translations.First().Language);
            }

            if (response.IsSuccessStatusCode)
            {
                List<WordWithTranslations>? result = await response.Content.ReadFromJsonAsync<List<WordWithTranslations>>();
                if (result is not null)
                {
                    var firstResult = result.FirstOrDefault();
                    if (firstResult is not null)
                    {
                        var updatedWord = UpdateTranslationInDb(wordWithTranslations, firstResult);
                        if (updatedWord is not null)
                            return Ok(updatedWord);
                        else
                            return BadRequest();
                    }
                }
                return Ok(new WordWithTranslations { DisplaySource = "" });         //Result is null or translations is empty => word did not found
            }
            else
            {
                return BadRequest("Error: " + response.StatusCode);
            }
        }

        private WordWithTranslations? UpdateTranslationInDb(WordWithTranslations updateFrom, WordWithTranslations updateTo)
        {
            var existingWord = _dictionaryDbContext.Words.FirstOrDefault(w => w.Id == updateFrom.Id);
            if (existingWord != null)
            {
                existingWord.Translations = _dictionaryDbContext.Translations.Where(t => t.WordId == updateFrom.Id).ToList();
                updateTo.Translations.ToList().ForEach(t => t.Language = updateFrom.Translations.First().Language);
                existingWord.DisplaySource = updateFrom.DisplaySource.ToLower();
                existingWord.Language = updateFrom.Language;
                var oldTranslations = existingWord.Translations.Select(t => t.DisplayTarget);
                var newTranslations = updateTo.Translations.Select(t => t.DisplayTarget);
                if (!oldTranslations.SequenceEqual(newTranslations))
                {
                    existingWord.Translations = updateTo.Translations;
                    _dictionaryDbContext.SaveChanges();
                }
                return existingWord;
            }
            else
            {
                return null;
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
                    request.Method = System.Net.Http.HttpMethod.Post;
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
