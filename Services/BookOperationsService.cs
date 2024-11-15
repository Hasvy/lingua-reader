﻿using Microsoft.AspNetCore.Components;
using Objects.Entities;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Books.TxtBook;
using Objects.Entities.Translator;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<IList<BookSection>> GetBookSections(Guid id)      //Epub
        {
            try
            {
                var bookSections = await _httpClient.GetFromJsonAsync<IList<BookSection>>($"api/BookSection/Get/{id}");
                return bookSections;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PdfBook> GetPdfBookText(Guid id)      //Pdf
        {
            try
            {
                var book = await _httpClient.GetFromJsonAsync<PdfBook>($"api/PdfBook/Get/{id}");
                return book;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TxtBook> GetTxtBookText(Guid id)      //Pdf
        {
            try
            {
                var book = await _httpClient.GetFromJsonAsync<TxtBook>($"api/TxtBook/Get/{id}");
                return book;
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

        public async Task<HttpResponseMessage> PostEpubBook(EpubBook book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/EpubBook/Post", book);
                //TODO Return some info
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<HttpResponseMessage> PostPdfBook(PdfBook book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/PdfBook/Post", book); ;
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<HttpResponseMessage> SaveTxtBook(TxtBook book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/TxtBook/Post", book); ;
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<HttpResponseMessage> ChangeBookLang(string newLanguage, Guid bookCoverId)
        {
            string apiUri = $"api/BookCover/Language/Put?newLanguage={newLanguage}&bookCoverId={bookCoverId}";
            var response = await _httpClient.PutAsync(apiUri, null);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteBook(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Book/Delete/{id}");
                return response;
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<EpubBook>();
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
