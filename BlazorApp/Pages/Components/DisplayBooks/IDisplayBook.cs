namespace BlazorApp.Pages.Components.DisplayBooks
{
    public interface IDisplayBook
    {
        public int ActualPageNumber { get; set; }
        public int PagesCount { get; set; }
        void NextPage();
        void PreviousPage();
        void ChangePage(int? pageNumber);
    }
}
