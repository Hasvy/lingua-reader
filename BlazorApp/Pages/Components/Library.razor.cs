using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Objects.Components.Library;
using Radzen;
using Services;
using System.IO;
using System.Text;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        //List<IBrowserFile> file = new();

        BookCover bookCover = new BookCover();
        List<BookCover>? userBooks = new List<BookCover>();
        private long maxFileSize = 1024 * 1024 * 10;
        private ElementReference fileInput;

        protected override async Task OnInitializedAsync()
        {
            bookCover.Title = "Title";
            bookCover.Author = "Author";
            bookCover.Format = BookFormat.pdf;
            userBooks.Add(bookCover);
            //userBooks.Add(new BookCover() { Title = "Book2" });
            //userBooks.Add(new BookCover() { Title = "Book3" });
            await base.OnInitializedAsync();
        }

        private async Task OpenBook(BookCover choosenBook)
        {
            //NavigationManager.NavigateTo("read/" + choosenBook.Id.ToString());
        }

        private async Task AddBook(InputFileChangeEventArgs e)
        {
            try
            {
                //In future, I can use JavaScript Interop to save and then read a file
                FileStream fileStream = new(Directory.GetCurrentDirectory() + "123.pdf", FileMode.Create);

                await e.File.OpenReadStream(maxFileSize).CopyToAsync(fileStream);
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
                    await SaveBook(pageContent);
                }
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
            //TODO Save a book to LocalStorage in browser
        }
    }
}
