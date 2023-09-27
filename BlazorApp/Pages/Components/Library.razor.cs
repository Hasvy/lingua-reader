using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Objects;
using Objects.Entities;
using Radzen;
using Services;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] AddBookService AddBookService { get; set; } = null!;
        //[Inject] EpubConverter epubConverter { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        private IList<BookCover> _userBooks = new List<BookCover>();
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _userBooks = (await BookOperationsService.GetBookCovers()).ToList();
            await base.OnInitializedAsync();
            _isLoading = false;
        }

        private async Task AddBookToDatabase(InputFileChangeEventArgs e)
        {
            string? fileExtension = new System.IO.FileInfo(e.File.Name).Extension;      //Get file extension
            Book? book = null;
            switch (fileExtension)
            {
                case ConstBookFormats.pdf:
                    await AddBookService.AddNewPdfBook(e);
                    break;
                case ConstBookFormats.epub:
                    book = await AddBookService.AddNewEpubBook(e);
                    break;
                case ConstBookFormats.fb2:
                    //TODO
                    break;
                case ConstBookFormats.mobi:
                    //TODO
                    break;
                default:
                    break;
            }

            if (book is not null)
            {
                _userBooks.Add(book.BookCover);
            }
        }

        private void OpenBook(BookCover choosenBook)
        {
            NavigationManager.NavigateTo("read/" + choosenBook.BookId);
        }

        private async Task DeleteBook(BookCover bookCover)
        {
            bool? confirm = await DialogService.Confirm("Do you want to delete the book?", "Confirm", new ConfirmOptions() { OkButtonText = "Yes", 
                                                                                                                             CancelButtonText = "No", 
                                                                                                                             AutoFocusFirstElement = false });
            if (confirm == true)
            {
                await BookOperationsService.DeleteBook(bookCover.BookId);
                _userBooks.Remove(bookCover);
            }
        }
    }
}
