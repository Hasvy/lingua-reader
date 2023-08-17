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

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;

        private List<BookCover> _userBooks = new List<BookCover>();
        private List<string> _pages = new List<string>();
        private long _maxFileSize = 1024 * 1024 * 10;       //10Mb

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

        private async Task AddBook(InputFileChangeEventArgs e)
        {
            try
            {
                //In future, I can use JavaScript Interop to save and then read a file. Also I can save all information in file, like drawio.
                FileStream fileStream = new(Directory.GetCurrentDirectory() + "123.pdf", FileMode.Create);

                await e.File.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);
                var fi = new System.IO.FileInfo("/123.pdf").Length;

                PdfReader pdfReader = new PdfReader("/123.pdf");
                PdfDocument pdfDoc = new PdfDocument(pdfReader);
                var pagesCount = pdfDoc.GetNumberOfPages();
                for (int page = 1; page <= pagesCount; page++)
                {
                    //TODO parse pdf file with saving format. At least titles and images
                    ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    //ITextExtractionStrategy strategy2 = new iText.Kernel.Pdf.Canvas.Parser.Listener.
                    //_pages.Add(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy));
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(pdfDoc.GetPage(page));
                    _pages.Add(strategy.GetResultantText());
                    await Task.Delay(1);
                    ProgressService.UpdateProgress(page * 100 / pagesCount);
                }
                //await SaveBook();
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
