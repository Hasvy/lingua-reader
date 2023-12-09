using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Objects.Constants;
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
        [MaxLength(9)]
        public string Language { get; set; } = ConstLanguages.Undefined;
        public string? CoverImage { get; set; }

        public Guid BookId { get; set; }        //Foreign key

        public BookCover()
        {
            Id = Guid.NewGuid();
        }
    }
}