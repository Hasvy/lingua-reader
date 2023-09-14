using Objects.Entities;
using System.Net.Http.Json;

namespace Services
{
    public class BookCoverService
    {
        private readonly HttpClient _httpClient;

        public BookCoverService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BookCover>> GetBookCovers()
        {
            try
            {
                var bookCovers = await _httpClient.GetFromJsonAsync<IEnumerable<BookCover>>("api/BookCover");
                return bookCovers;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
