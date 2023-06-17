using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Objects.Components.Library;
using Radzen;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        EventCallback callback;
        //[Inject]
        //IHostingEnvironment Environment { get; set; }

        List<IBrowserFile> file = new();

        BookCover bookCover = new BookCover();
        List<BookCover>? userBooks = new List<BookCover>();

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

        private async Task AddBook()                //How to save books?
        {
            try
            {
                var path = "C:\\Projects\\LinguaReader";
                //Dont save files, but translate them to some format, and save it.


                //await using FileStream fileStream = new(path, FileMode.Create);
                
                //await file.First().OpenReadStream().CopyToAsync(fileStream);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
