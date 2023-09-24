using Objects.Entities;
using System.Net.Http.Json;

namespace Services
{
    public class BookOperationsService
    {
        private readonly HttpClient _httpClient;

        public BookOperationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BookCover>> GetBookCovers()
        {
            try
            {
                var bookCovers = await _httpClient.GetFromJsonAsync<IEnumerable<BookCover>>("api/BookCover/Get");
                return bookCovers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task PostBook(Book book)
        {
            try
            {
                var addedBook = await _httpClient.PostAsJsonAsync("api/Book/Post", book);
                //TODO Return some info
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteBook(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Book/Delete/{id}");

                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<Book>();
                //}
                //return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
