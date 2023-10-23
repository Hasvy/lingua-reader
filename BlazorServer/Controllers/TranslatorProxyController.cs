using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Objects.Entities.Translator;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

namespace BlazorServer.Controllers
{

    public class TranslatorProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static readonly string key = "e3e05bc92ae54af883a388e21cedb21f";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string location = "westeurope";

        public TranslatorProxyController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        [Route("api/Proxy/TranslateWord/{Word}")]
        public async Task<ActionResult<TranslatorWordResponse?>> TranslateWord(string word)
        {
            string route = "dictionary/lookup?api-version=3.0&from=en&to=cs";
            //string wordToTranslate = "Hello";
            object[] body = new object[] { new { Text = word } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    List<TranslatorWordResponse>? result = await response.Content.ReadFromJsonAsync<List<TranslatorWordResponse>>();
                    return Ok(result.FirstOrDefault());      //TODO if result is null or translations is empty => word did not found
                }
                else
                {
                    return BadRequest("Error: " + response.StatusCode);
                }
            }
        }

        [HttpGet]
        [Route("api/Proxy/TranslateText")]
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
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", location);

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
