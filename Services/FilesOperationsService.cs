using Objects.Entities.Books.EpubBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FilesOperationsService
    {
        private readonly HttpClient _httpClient;

        public FilesOperationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<HttpResponseMessage> PostBookFile(byte[] bookBytes)     //Maybe send bytes like when GetBookFile with JsonSerializer
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("api/BookFile/Post", bookBytes);
        //        return response;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<string> GetBookFile(Guid id)
        {
            try
            {   //Or another variant, return string in EpubBook object, book.BookContentFile parameter
                //Or with JsonSerializer.SerializeToUtf8Bytes(bytes)
                var book = await _httpClient.GetFromJsonAsync<EpubBook>($"api/BookFile/Get/{id}");
                return book.BookContentFile;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
