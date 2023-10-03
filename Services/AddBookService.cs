using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Objects.Entities;
using VersOne.Epub;
using Objects;
using System.IO;
using System.Diagnostics;

namespace Services
{
    /// <summary>
    /// This service adds a book to library. Every book format has its own method.
    /// </summary>
    public class AddBookService
    {
        private readonly BookOperationsService _bookOperationsService;
        private readonly HtmlParserService _htmlParserService;
        private readonly ProgressService _progressService;
        private long _maxFileSize = 1024 * 1024 * 10;       //10Mb
        public AddBookService(BookOperationsService bookOperationsService, ProgressService progressService, HtmlParserService htmlParserService)
        {
            _bookOperationsService = bookOperationsService;
            _progressService = progressService;
            _htmlParserService = htmlParserService;
        }

        #region Epub book processing
        public async Task<Book?> AddNewEpubBook(InputFileChangeEventArgs e)
        {
            
            MemoryStream memoryStream = await GetMemoryStreamFromInput(e);

            //EpubBook epubBook = await EpubReader.ReadBookAsync(memoryStream);       //TODO add ePubSharp library like additional try to read a book when versone cant read a book, maybe it will read Sheakspere book file
            EpubBookRef epubBook = await EpubReader.OpenBookAsync(memoryStream);
            Book book = await SetBookData(epubBook);
            book.BookContentFile = Convert.ToBase64String(memoryStream.ToArray());
            var response = await _bookOperationsService.PostBook(book);

            if (response.IsSuccessStatusCode)
            {
                return book;
            }
            else
            {
                return null;
                //Notification and log
            }
        }

        private async Task<MemoryStream> GetMemoryStreamFromInput(InputFileChangeEventArgs e)
        {
            var ms = new MemoryStream();        //Possible memory leak?
            await e.File.OpenReadStream(_maxFileSize).CopyToAsync(ms);
            return ms;

        }

        private async Task<Book> SetBookData(EpubBookRef epubBook)     //Assign epubBook data to Book object
        {
            Book book = new Book
            {
                BookCover = new BookCover
                {
                    Author = epubBook.Author,
                    Title = epubBook.Title,
                    Description = epubBook.Description,
                    Format = ConstBookFormats.epub
                }
            };
            book.BookCover.BookId = book.Id;
            //book.Content = await SetBookContent(epubBook, book);
            book.Sections = await SetBookSections(epubBook, book);
            book.SectionsCount = book.Sections.Count();
            return book;
        }

        private async Task<List<BookSection>> SetBookSections(EpubBookRef epubBook, Book book)
        {
            var sectionList = new List<BookSection>();
            int index = 0;
            //var epubBookref = await EpubReader.OpenBookAsync(epubBook);     //work with epubbook or epubbookref

            foreach (var item in epubBook.Content.Html.Local)           //I use Local file. What is remote file?
            {
                var bookSection = new BookSection();
                bookSection.Text = await _htmlParserService.Parse(item.ReadContentAsync().Result, epubBook);
                bookSection.OrderNumber = index;
                bookSection.BookId = book.Id;
                List<Page> pages = new List<Page> { new Page { Number = 1, SectionId = bookSection.Id, Text = "Text" } };           //Pages //Change if I will use them
                bookSection.Pages = pages;
                sectionList.Add(bookSection);
                index++;
            }
            return sectionList;
        }

        private async Task<List<BookContent>> SetBookContent(EpubBookRef epubBook, Book book)
        {
            var contentList = new List<BookContent>();

            foreach (var item in epubBook.Content.Css.Local)
            {
                var bookContent = new BookContent();
                bookContent.Type = ContentType.css;
                bookContent.Content = await item.ReadContentAsync();
                bookContent.BookId = book.Id;
                contentList.Add(bookContent);
                //I can't add a file storage on server. What after I upload file on server? How work with it?
                //The file will be on server, but I need it on a client, so I cant work with the file.
                //BLOB in database then wirte it in memory - for images
                //css and images to datauri string and then apply it to code +
            }
            return contentList;
        }

        #endregion

        #region Pdf book processing

        private List<string> _pages = new List<string>();
        public async Task AddNewPdfBook(InputFileChangeEventArgs e)
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
                    _progressService.UpdateProgress(page * 100 / pagesCount);
                }
                SaveBook();
                pdfDoc.Close();
                pdfReader.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveBook()
        {
            Book book = new Book();
            book.BookCover = new BookCover();
            book.BookCover.Title = "Mars";
            book.BookCover.Author = "Breadbury";

            //_userBooks.Add(book.BookCover);

            string cover = JsonConvert.SerializeObject(book.BookCover);
            string text = JsonConvert.SerializeObject(_pages);
            //await localStorage.SetItemAsync(book.BookCover.Id.ToString("N"), cover);
            //await localStorage.SetItemAsync(book.BookCover.TextId.ToString("D"), text);
        }
        #endregion
    }
}