using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Objects;
using Objects.Entities;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Radzen;
using Services;

namespace BlazorApp.Pages.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] AddBookService AddBookService { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        [Inject] ILocalStorageService LocalStorageService { get; set; } = null!;

        private IList<BookCover> _userBooks = new List<BookCover>();
        private bool _isLoading;
        private bool _isUploading;

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
            AbstractBook? book = null;
            switch (fileExtension)
            {
                case ConstBookFormats.pdf:
                    book = await AddBookService.AddNewPdfBook(e);
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

        private async void OpenBook(BookCover choosenBook)
        {
            if (choosenBook.Language == ConstLanguages.Undefined)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Please specify language of the book");
            }
            var userMainLang = await LocalStorageService.GetItemAsStringAsync("UserMainLang");
            if (string.IsNullOrEmpty(userMainLang))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Please specify your language in settings");
                return;
            }
            await LocalStorageService.SetItemAsStringAsync("bookFormat", choosenBook.Format);
            NavigationManager.NavigateTo("read/" + choosenBook.BookId + $"?lang={choosenBook.Language}");
            //NavigationManager.NavigateTo("read/" + choosenBook.BookId + $"?bookFormat={choosenBook.Format}", true);
        }

        void Notify(NotificationMessage message)
        {
            NavigationManager.NavigateTo(Routes.Settings);
        }

        private async Task DeleteBook(BookCover bookCover)
        {
            bool? confirm = await DialogService.Confirm("Do you want to delete the book?", "Confirm", new ConfirmOptions() { OkButtonText = "Yes", 
                                                                                                                             CancelButtonText = "No", 
                                                                                                                             AutoFocusFirstElement = false });
            if (confirm == true)
            {
                var response = await BookOperationsService.DeleteBook(bookCover.BookId);
                if (response.IsSuccessStatusCode)
                {
                    _userBooks.Remove(bookCover);
                }
            }
        }

        private async Task ChangeBookLang(BookCover bookCover)
        {
            await BookOperationsService.ChangeBookLang(bookCover.Language, bookCover.Id);
        }

        private string GetFlagPath(string language)
        {
            switch (language)
            {
                case ConstLanguages.Czech: return "img/czech-republic-32.png";
                case ConstLanguages.English: return "img/great-britain-32.png";
                case ConstLanguages.German: return "img/germany-32.png";
                case ConstLanguages.Russian: return "img/russian-federation-32.png";
                case ConstLanguages.Italian: return "img/italy-32.png";
                case ConstLanguages.Spanish: return "img/spain-flag-32.png";
                default:
                    return "img/question-32.png";
            }
        }
    }
}
