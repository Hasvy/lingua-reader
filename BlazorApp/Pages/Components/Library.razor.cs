﻿using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Radzen;
using Services;
using System.IO;
using System.Text;
using Objects;
using System.ComponentModel;
using Microsoft.JSInterop;
using iText.Commons.Utils;
using VersOne.Epub;
using Objects.Entities;
using System.Net;
using static System.Collections.Specialized.BitVector32;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp;
using System.Threading;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;
        [Inject] EpubConverter epubConverter { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;

        private List<BookCover> _userBooks = new List<BookCover>();
        private List<string> _pages = new List<string>();
        private List<string> _contentFiles = new List<string>();
        private long _maxFileSize = 1024 * 1024 * 10;       //10Mb
        private IEnumerable<BookCover> _bookCovers;

        string text = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _bookCovers = await BookOperationsService.GetBookCovers();
            _userBooks.AddRange(_bookCovers);

            //TODO change saving books to database

            for(int i = 0; i < await localStorage.LengthAsync(); i++)
            {
                try
                {
                    string? key = await localStorage.KeyAsync(i);
                    Guid result = new Guid();

                    if (string.IsNullOrEmpty(key) || !Guid.TryParseExact(key, "N", out result))
                    {
                        continue;
                    }
                    var stringCover = await localStorage.GetItemAsync<string>(key);
                    var cover = JsonConvert.DeserializeObject<BookCover>(stringCover);
                    
                    if (cover != null)
                    {
                        _userBooks.Add(cover);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            await base.OnInitializedAsync();
        }

        private async Task OpenBook(BookCover choosenBook)
        {
            NavigationManager.NavigateTo("read/" + choosenBook.BookId);
        }

        private async Task AddBookToDatabase(InputFileChangeEventArgs e)
        {
            string? fileExtension = new System.IO.FileInfo(e.File.Name).Extension;
            switch (fileExtension)
            {
                case ConstBookFormats.pdf:
                    await AddPdfBook(e);
                    break;
                case ConstBookFormats.epub:
                    await AddNewEpubBook(e);
                    break;
                case ConstBookFormats.fb2:
                    //TODO
                    break;
                default:
                    break;
            }
        }

        private async Task AddNewEpubBook(InputFileChangeEventArgs e)
        {
            EpubBook epubBook = await GetEpubFormat(e);
            Book book = new Book
            {
                BookCover = new BookCover
                {
                    Author = epubBook.Author,
                    Title = epubBook.Title,
                    Description = epubBook.Description,
                    Format = BookFormat.epub.ToString()
                }
            };
            book.BookCover.BookId = book.Id;
            book.Sections = await ProcessEpubBook(book, epubBook);

            var response = await BookOperationsService.PostBook(book);

            if (response.IsSuccessStatusCode)
            {
                _userBooks.Add(book.BookCover);
            }
            
            //NavigationManager.NavigateTo(Routes.Reading, true);
        }

        private async Task<EpubBook> GetEpubFormat(InputFileChangeEventArgs e)          //ConvertToHtml
        {
            string tmpFileName = "123.epub";                                        //Optimize that code{
            string tmpFilePath = Directory.GetCurrentDirectory() + tmpFileName;

            using(var ms = new MemoryStream())
            {
                await e.File.OpenReadStream(_maxFileSize).CopyToAsync(ms);
                byte[] data = ms.ToArray();
                File.WriteAllBytes(tmpFilePath, data);
            }

            return await EpubReader.ReadBookAsync("/123.epub");
        }

        private async Task<IEnumerable<BookSection>> ProcessEpubBook(Book book, EpubBook epubBook)
        {
            var list = new List<BookSection>();
            int index = 0;
            foreach (var item in epubBook.Content.Html.Local)           //What is remote file?
            {
                var bookSection = new BookSection();
                bookSection.Text = item.Content;
                bookSection.OrderNumber = index;
                bookSection.BookId = book.Id;

                //Pages //Change
                List<Page> pages = new List<Page> { new Page { Number = 1, SectionId = bookSection.Id, Text = "Text" } };
                bookSection.Pages = pages;

                list.Add(bookSection);
                index++;
            }
            foreach (var item in epubBook.Content.Css.Local)
            {
                //Add file storage on server
            }

            book.SectionsCount = list.Count;
            return list;
        }

        private async Task<List<IElement>> ParseBookSectionToList(string section)
        {
            var parser = new HtmlParser();
            IHtmlDocument _htmlDocument = await parser.ParseDocumentAsync(section);
            var head = _htmlDocument.Head.OuterHtml;
            var bodyElements = _htmlDocument.Body.Children.ToList();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            IDocument iframeDocument = await context.OpenNewAsync();
            iframeDocument.Head.OuterHtml = _htmlDocument.Head.OuterHtml;
            iframeDocument.Body.OuterHtml = _htmlDocument.Body.OuterHtml;
            iframeDocument.Body.InnerHtml = "";

            return bodyElements;
        }

        private async Task AddPdfBook(InputFileChangeEventArgs e)
        {
            try
            {
                //In future, I can use JavaScript Interop to save and then read a file. Also I can save all information in file, like drawio.
                await using FileStream fileStream = new(Directory.GetCurrentDirectory() + "123.pdf", FileMode.Create);

                await e.File.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);
                var fi = new System.IO.FileInfo("/123.pdf").Length;

                PdfReader pdfReader = new PdfReader("/123.pdf");
                PdfDocument pdfDoc = new PdfDocument(pdfReader);
                var pagesCount = pdfDoc.GetNumberOfPages();
                for (int page = 1; page <= pagesCount; page++)
                {
                    //TODO parse pdf file with saving format. At least titles and images
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    //ITextExtractionStrategy strategy2 = new iText.Kernel.Pdf.Canvas.Parser.Listener.
                    _pages.Add(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy));
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(pdfDoc.GetPage(page));
                    _pages.Add(strategy.GetResultantText());
                    await Task.Delay(1);
                    ProgressService.UpdateProgress(page * 100 / pagesCount);
                }
                await SaveBook();
                pdfDoc.Close();
                pdfReader.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SaveBook()
        {
            Book book = new Book();
            book.BookCover = new BookCover();
            book.BookCover.Title = "Mars";
            book.BookCover.Author = "Breadbury";

            _userBooks.Add(book.BookCover);

            string cover = JsonConvert.SerializeObject(book.BookCover);
            string text = JsonConvert.SerializeObject(_pages);
            await localStorage.SetItemAsync(book.BookCover.Id.ToString("N"), cover);
            //await localStorage.SetItemAsync(book.BookCover.TextId.ToString("D"), text);
        }

        private async Task DeleteBook(BookCover bookCover)
        {
            bool? confirm = await DialogService.Confirm("Do you want to delete the book?", "Confirm", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirm == true)
            {
                await BookOperationsService.DeleteBook(bookCover.BookId);
                _userBooks.Remove(bookCover);       //Do something with that
            }
        }
    }
}
