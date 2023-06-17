namespace Objects.Components.Library
{
    public class Book
    {
        public BookCover BookCover { get; set; }
        public string? Text { get; set; }            //Find a way to save a text of a book, is string ok?

    }

    public enum BookFormat
    {
        pdf,
        epub
    }
}