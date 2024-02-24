using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.DisplayBooks
{
    public interface IDisplayBook
    {
        public string BookId { get; set; }
        public string BookLanguage { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PagesCount { get; set; }
        void NextPage();
        void PreviousPage();
        void JumpToPage(int? pageNumber);
    }
}
