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

        private List<BookCover>? _userBooks = new List<BookCover>();
        private long _maxFileSize = 1024 * 1024 * 10;

        protected override async Task OnInitializedAsync()
        {
            for(int i = 0; i < await localStorage.LengthAsync(); i++)
            {
                try
                {
                    string? key = await localStorage.KeyAsync(i);
                    Guid result = new Guid();

                    if (string.IsNullOrEmpty(key) || !Guid.TryParse(key, out result))
                    {
                        continue;
                    }
                    var stringCover = await localStorage.GetItemAsync<string>(key);
                    var cover = JsonConvert.DeserializeObject<BookCover>(stringCover);
                    
                    _userBooks.Add(cover);
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
            NavigationManager.NavigateTo("read/" + choosenBook.Id.ToString());
        }

        private async Task AddBook(InputFileChangeEventArgs e)
        {
            try
            {
                //In future, I can use JavaScript Interop to save and then read a file
                FileStream fileStream = new(Directory.GetCurrentDirectory() + "123.pdf", FileMode.Create);

                await e.File.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);
                var fi = new System.IO.FileInfo("/123.pdf").Length;

                PdfReader pdfReader = new PdfReader("/123.pdf");
                PdfDocument pdfDoc = new PdfDocument(pdfReader);
                string pageContent = string.Empty;
                var pages = pdfDoc.GetNumberOfPages();
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    //TODO fix processing file to comfort save and read then
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    pageContent += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                }
                await SaveBook(pageContent);
                pdfDoc.Close();
                pdfReader.Close();
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveBook(string content)                 //How to save books?
        {
            Book book = new Book();
            book.BookCover = new BookCover();
            book.BookCover.Title = "Mars";
            book.BookCover.Author = "Breadbury";
            book.Text = content;

            string cover = JsonConvert.SerializeObject(book.BookCover);
            await localStorage.SetItemAsync(book.BookCover.Id.ToString(), cover);
        }
    }
}
