using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Objects.Entities
{
    public class BookCover
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string Format { get; set; } = null!;

        public Guid BookId { get; set; }
        public virtual Book Book { get; set; }

        public BookCover()
        {
            Id = new Guid();
        }

    }
}