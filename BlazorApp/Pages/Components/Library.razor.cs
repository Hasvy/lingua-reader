using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Objects.Components.Library;
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

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;
        [Inject] EpubConverter epubConverter { get; set; } = null!;

        private List<BookCover> _userBooks = new List<BookCover>();
        private List<string> _pages = new List<string>();
        private List<string> _contentFiles = new List<string>();
        private long _maxFileSize = 1024 * 1024 * 10;       //10Mb

        string text = string.Empty;

        protected override async Task OnInitializedAsync()
        {
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
            NavigationManager.NavigateTo("read/" + choosenBook.Id.ToString("N"));
        }

        private async Task FileTypeSwitch(InputFileChangeEventArgs e)
        {
            string? fileExtension = new System.IO.FileInfo(e.File.Name).Extension;
            switch (fileExtension)
            {
                case ConstBookFormats.pdf:
                    await AddPdfBook(e);
                    break;
                case ConstBookFormats.epub:
                    await AddEpubBook(e);
                    break;
                case ConstBookFormats.fb2:
                    //TODO
                    break;
                default:
                    break;
            }
        }

        private async Task AddEpubBook(InputFileChangeEventArgs e)          //ConvertToHtml
        {
            string tmpFileName = "123.epub";                                        //Optimize that code{
            string tmpFilePath = Directory.GetCurrentDirectory() + tmpFileName;
            //await using FileStream fileStream = new(tmpFilePath, FileMode.Create);
            //await e.File.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);

            using(var ms = new MemoryStream())
            {
                await e.File.OpenReadStream(_maxFileSize).CopyToAsync(ms);
                byte[] data = ms.ToArray();
                File.WriteAllBytes(tmpFilePath, data);
                //await JS.InvokeVoidAsync("downloadFile", tmpFileName, data);      //}
            }

            EpubBook epubBook = await EpubReader.ReadBookAsync("/123.epub");
            //string? htmlFile = epubConverter.Convert("/123.epub");

            if (epubBook != null)
            {
                await SaveEpubBook(epubBook);
            }
        }

        private async Task SaveEpubBook(EpubBook epubBook)
        {
            Book book = new Book();
            book.BookCover = new BookCover();
            book.BookCover.Title = epubBook.Title;
            book.BookCover.Author = epubBook.Author;
            book.BookCover.Description = epubBook.Description;
            book.BookCover.Format = ConstBookFormats.epub;

            _userBooks.Add(book.BookCover);

            foreach (var item in epubBook.Content.Html.Local)           //What is remote file?
            {
                _contentFiles.Add(item.Content);
            }
            string cover = JsonConvert.SerializeObject(book.BookCover);
            string text = JsonConvert.SerializeObject(_contentFiles);
            await localStorage.SetItemAsync(book.BookCover.Id.ToString("N"), cover);
            await localStorage.SetItemAsync(book.BookCover.TextId.ToString("D"), text);
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
            await localStorage.SetItemAsync(book.BookCover.TextId.ToString("D"), text);
        }

        private async Task DeleteBook(BookCover bookCover)
        {
            bool? confirm = await DialogService.Confirm("Do you want to delete the book?", "Confirm", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirm == true)
            {
                _userBooks.Remove(bookCover);
                await localStorage.RemoveItemAsync(bookCover.Id.ToString("N"));
                await localStorage.RemoveItemAsync(bookCover.TextId.ToString("D"));
            }
        }
    }
}
