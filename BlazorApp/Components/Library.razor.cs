using Blazored.LocalStorage;
using iText.Forms.Xfdf;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Objects.Constants;
using Objects.Entities;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Radzen;
using Services;
using Services.Authentication;

namespace BlazorApp.Components
{
    public partial class Library : ComponentBase
    {
        [Inject] AddBookService AddBookService { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] DialogService DialogService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        [Inject] AuthStateProvider AuthStateProvider { get; set; } = null!;
        [Inject] UserService UserService { get; set; } = null!;
        [Inject] ILocalStorageService LocalStorageService { get; set; } = null!;
        //[Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;

        private IList<BookCover> _userBooks = new List<BookCover>();
        private bool _isLoading;
        private bool _isUploading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            //HttpInterceptorService.RegisterEvent();
            _userBooks = (await BookOperationsService.GetBookCovers()).ToList();
            _isLoading = false;
            
            await base.OnInitializedAsync();
        }

        private async Task AddNewBook(InputFileChangeEventArgs e)
        {
            DialogService.Open<LoadingProgress>($"Uploading", new Dictionary<string, object>() { }, new DialogOptions() { ShowClose = false, Width = "fit-content", Height = "fit-content" });
            string? fileExtension = new System.IO.FileInfo(e.File.Name).Extension;      //Get file extension
            AbstractBook? book = null;
            try
            {
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
                        DialogService.Close();
                        break;
                }

                if (book is not null)   //ToDo MB get books from db again instead of separating
                {
                    _userBooks.Add(book.BookCover);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                DialogService.Close();
            }
        }

        private async void OpenBook(BookCover choosenBook)
        {
            if (choosenBook.Language == ConstLanguages.Undefined)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Please specify a language of the book");
                return;
            }
            //var userMainLang = await LocalStorageService.GetItemAsStringAsync("UserMainLang");
            //if (string.IsNullOrEmpty(userMainLang))
            //{
            //    NotificationService.Notify(NotificationSeverity.Error, "Please specify your language in settings");
            //    return;
            //}
            string userMainLang = await UserService.GetUserMainLanguage();
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
    }
}
