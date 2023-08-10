using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Objects.Components.Library;
using Radzen;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        //List<IBrowserFile> file = new();

        BookCover bookCover = new BookCover();
        List<BookCover>? userBooks = new List<BookCover>();
        private long maxFileSize = 1024 * 1024 * 1;

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
            NavigationManager.NavigateTo("read/" + choosenBook.Id.ToString());
        }

        private async Task AddBook(InputFileChangeEventArgs e)                //How to save books?
        {
            try
            {
                var file = e.File;
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream()), "file", file.Name);
                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("/api/files", content);
                string path = "C:\\Projects\\LinguaReader\\BookStorage\\" + e.File.Name;
                await using MemoryStream stream = new MemoryStream();
                await file.OpenReadStream(maxFileSize).CopyToAsync(stream);



                //await using FileStream fileStream = new(path, FileMode.Create);
                //await file.OpenReadStream(maxFileSize).CopyToAsync(fileStream);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
