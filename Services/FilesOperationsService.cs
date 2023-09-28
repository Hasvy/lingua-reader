using Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
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

        public async Task<byte[]> GetBookFile(Guid id)
        {
            try
            {   //Or another variant, return string in Book object, book.BookContentFile parameter
                var bytes = await _httpClient.GetFromJsonAsync<byte[]>($"api/BookFile/Get/{id}");
                return bytes;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
