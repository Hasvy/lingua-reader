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

        public async Task<IEnumerable<BookSection>> GetBookSections(Guid id)
        {
            try
            {
                var bookSections = await _httpClient.GetFromJsonAsync<IEnumerable<BookSection>>($"api/BookSection/Get/{id}");
                return bookSections;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BookContent>> GetBookContent(Guid id)
        {
            try
            {
                var bookContent = await _httpClient.GetFromJsonAsync<IEnumerable<BookContent>>($"api/BookContent/Get/{id}");
                return bookContent;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<HttpResponseMessage> PostBook(Book book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Book/Post", book);
                //TODO Return some info
                return response;
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
