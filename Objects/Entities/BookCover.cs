using System.ComponentModel;
using System.Text.Json.Serialization;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;

namespace Objects.Entities
{
    public class BookCover
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string Format { get; set; } = null!;
        public Guid BookId { get; set; }        //Foreign key

        public BookCover()
        {
            Id = Guid.NewGuid();
        }
    }
}