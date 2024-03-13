using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.StyledXmlParser.Jsoup.Parser;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Objects.Constants;
using Objects.Entities;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Books.TxtBook;
using Radzen;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Text;
using VersOne.Epub;
using VersOne.Epub.Schema;

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
        private readonly NotificationService _notificationService;
        private readonly IJSRuntime _JS;
        private long _maxFileSize = 1024 * 1024 * 10;       //10Mb
        public AddBookService(BookOperationsService bookOperationsService, ProgressService progressService, HtmlParserService htmlParserService, IJSRuntime jSRuntime, NotificationService notificationService)
        {
            _bookOperationsService = bookOperationsService;
            _progressService = progressService;
            _htmlParserService = htmlParserService;
            _JS = jSRuntime;
            _notificationService = notificationService;
        }

        #region Epub book processing
        public async Task<Objects.Entities.Books.EpubBook.EpubBook?> AddNewEpubBook(InputFileChangeEventArgs e)
        {
            _progressService.UpdateProgress(0);
            MemoryStream? memoryStream = await GetMemoryStreamFromInput(e);
            if (memoryStream is null)
                return null;
            //EpubBook epubBook = await EpubReader.ReadBookAsync(memoryStream);       //TODO add ePubSharp library like additional try to read a book when versone cant read a book, maybe it will read Sheakspere book file
            EpubBookRef epubBook = await EpubReader.OpenBookAsync(memoryStream);
            Objects.Entities.Books.EpubBook.EpubBook book = await SetBookData(epubBook);
            book.BookContentFile = Convert.ToBase64String(memoryStream.ToArray());
            var response = await _bookOperationsService.PostEpubBook(book);

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

        private async Task<Objects.Entities.Books.EpubBook.EpubBook> SetBookData(EpubBookRef epubBook)     //Assign epubBook data to EpubBook object
        {
            Objects.Entities.Books.EpubBook.EpubBook book = new Objects.Entities.Books.EpubBook.EpubBook
            {
                BookCover = new BookCover
                {
                    Author = epubBook.Author,
                    Title = epubBook.Title,
                    Description = epubBook.Description,
                    Format = ConstBookFormats.epub,
                }
            };
            //epubBook.Content.NavigationHtmlFile. or GetNavigation      //TODO nav file
            book.BookCover.CoverImage = await ResizeBookCoverImage(epubBook);
            book.BookCover.Language = GetBookLanguage(epubBook);
            book.BookCover.BookId = book.Id;
            book.Sections = await SetBookSections(epubBook, book);
            book.SectionsCount = book.Sections.Count();
            return book;
        }

        private async Task<List<BookSection>> SetBookSections(EpubBookRef epubBook, Objects.Entities.Books.EpubBook.EpubBook book)
        {
            var sectionList = new List<BookSection>();
            int index = 0;
            var readingOrder = await epubBook.GetReadingOrderAsync();
            //var epubBookref = await EpubReader.OpenBookAsync(epubBook);     //work with epubbook or epubbookref

            foreach (var item in readingOrder)           //I use Local file. What is remote file?
            {
                var bookSection = new BookSection();
                bookSection.Text = await _htmlParserService.Parse(item.ReadContentAsync().Result, epubBook);
                bookSection.OrderNumber = index;
                bookSection.EpubBookId = book.Id;
                sectionList.Add(bookSection);
                index++;
                await UpdateProgress(index, readingOrder.Count());
            }
            return sectionList;
        }

        private async Task<string?> ResizeBookCoverImage(EpubBookRef epubBook)
        {
            byte[]? coverImage = await epubBook.ReadCoverAsync();
            string? resizedImage = null;

            if (coverImage is not null)
            {
                int newHeight = 200;
                int newWidth = 200;
                using (MemoryStream stream = new MemoryStream(coverImage))
                {
                    using (Image image = Image.Load(stream))
                    {
                        int originalWidth = image.Width;
                        int originalHeight = image.Height;
                        double widthRatio = (double)newWidth / originalWidth;
                        double heightRatio = (double)newHeight / originalHeight;
                        double ratio = Math.Min(widthRatio, heightRatio);
                        int targetWidth = (int)(originalWidth * ratio);
                        int targetHeight = (int)(originalHeight * ratio);

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(newWidth, newHeight),
                            Mode = ResizeMode.Max
                        }));

                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            image.Save(outputStream, new JpegEncoder());
                            byte[] resizedImageBytes = outputStream.ToArray();
                            resizedImage = Convert.ToBase64String(resizedImageBytes);
                        }
                    }
                }
            }
            return resizedImage;
        }

        private string GetBookLanguage(EpubBookRef epubBook)
        {
            EpubMetadataLanguage? metadata = epubBook.Schema.Package.Metadata.Languages.FirstOrDefault();
            if (metadata is not null)
            {
                return ConstLanguages.DesiredLanguagesSet.FirstOrDefault(l => l == metadata.Language, ConstLanguages.Undefined);
            }
            else
            {
                return ConstLanguages.Undefined;
            }
        }

        //private async Task<List<BookContent>> SetBookContent(EpubBookRef epubBook, Objects.Entities.Books.EpubBook.EpubBook book)
        //{
        //    var contentList = new List<BookContent>();

        //    foreach (var item in epubBook.Content.Css.Local)
        //    {
        //        var bookContent = new BookContent();
        //        bookContent.Type = ContentType.css;
        //        bookContent.Content = await item.ReadContentAsync();
        //        bookContent.BookId = book.Id;
        //        contentList.Add(bookContent);
        //        //I can't add a file storage on server. What after I upload file on server? How work with it?
        //        //The file will be on server, but I need it on a client, so I cant work with the file.
        //        //BLOB in database then wirte it in memory - for images
        //        //css and images to datauri string and then apply it to code +
        //
        //        //UPD. Send file from server as byte array? Or at least images
        //    }
        //    return contentList;
        //}

        #endregion

        #region Pdf book processing

        //private List<string> _pages = new List<string>();
        public async Task<PdfBook?> AddNewPdfBook(InputFileChangeEventArgs e)
        {
            _progressService.UpdateProgress(0);
            MemoryStream? memoryStream = await GetMemoryStreamFromInput(e);
            if (memoryStream is null)
                return null;
            memoryStream.Seek(0, SeekOrigin.Begin);

            iText.Kernel.Pdf.PdfReader reader = new iText.Kernel.Pdf.PdfReader(memoryStream);
            iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader);

            //memoryStream.Seek(0, SeekOrigin.Begin);
            //string base64 = Convert.ToBase64String(memoryStream.ToArray());

            //List<string> _pages = new List<string>();
            var pagesCount = pdfDoc.GetNumberOfPages();
            //PdfResources resourses = pdfDoc.GetPage(1).GetResources();
            Encoding encoding = Encoding.UTF8;
            ITextExtractionStrategy strategy = new CustomTextExtractionStrategy(encoding);
            //ITextExtractionStrategy strategy2 = new SimpleTextExtractionStrategy();

            for (int pageNumber = 1; pageNumber <= pagesCount; pageNumber++)
            {
                PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);    //I will renew the processor with every page, otherwise start and end of a segment in strategy
                var page = pdfDoc.GetPage(pageNumber);                              //will create a very short interval, so the code will stop create new lines, I don't know why.
                processor.ProcessPageContent(page);
                await UpdateProgress(pageNumber, pagesCount);
            }

            PdfBook book = SaveBookData(pdfDoc);
            book.Text = strategy.GetResultantText();
            //foreach (var item in _pages)
            //{
            //    book.Text += item;
            //}
            var response = await _bookOperationsService.PostPdfBook(book);      //Here will add 

            if (response.IsSuccessStatusCode)
            {
                return book;
            }
            else
            {
                return null;
            }
        }

        private PdfBook SaveBookData(iText.Kernel.Pdf.PdfDocument pdfDoc)
        {
            PdfBook book = new PdfBook();
            book.BookCover = new BookCover();
            book.BookCover.Title = pdfDoc.GetDocumentInfo().GetTitle();
            book.BookCover.Author = pdfDoc.GetDocumentInfo().GetAuthor();
            book.BookCover.Format = ConstBookFormats.pdf;
            book.BookCover.BookId = book.Id;

            return book;
        }
        #endregion

        public async Task<TxtBook?> AddNewTxtBook(InputFileChangeEventArgs e)
        {
            _progressService.UpdateProgress(0);
            TxtBook book = new TxtBook();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("""<div id="txt-content">""");
            using (StreamReader reader = new StreamReader(e.File.OpenReadStream(), Encoding.UTF8))
            {
                string? line;
                int totalBytesRead = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    totalBytesRead += Encoding.UTF8.GetByteCount(line);
                    if (line == string.Empty)
                        stringBuilder.Append("<br/>");
                    else
                        stringBuilder.Append("<p>" + line + "</p>");
                    await UpdateProgress(totalBytesRead, (int)e.File.Size);
                }
            }

            stringBuilder.AppendLine("</div>");
            book.Text = stringBuilder.ToString();
            book.BookCover = new BookCover();
            book.BookCover.Format = ConstBookFormats.txt;
            book.BookCover.Title = Path.GetFileNameWithoutExtension(e.File.Name);
            book.BookCover.BookId = book.Id;

            var response = await _bookOperationsService.SaveTxtBook(book);

            if (response.IsSuccessStatusCode)
            {
                return book;
            }
            else
            {
                return null;
            }
        }

        private async Task<MemoryStream?> GetMemoryStreamFromInput(InputFileChangeEventArgs e)
        {
            var ms = new MemoryStream();
            if (_maxFileSize < e.File.Size)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Maximum allowed file size is 10 Mb");
                return null;
            }
            await e.File.OpenReadStream(_maxFileSize).CopyToAsync(ms);
            return ms;
        }

        private async Task UpdateProgress(int progress, int taskCount)
        {
            await Task.Delay(1);
            _progressService.GetCancellationToken().ThrowIfCancellationRequested();
            _progressService.UpdateProgress(progress * 100 / taskCount);
        }
    }
}