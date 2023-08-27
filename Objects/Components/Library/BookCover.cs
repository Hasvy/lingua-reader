using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Objects.Components.Library
{
    public class BookCover
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string Format { get; set; } = null!;
        //public BookFormat Format { get; set; }
        public Guid TextId { get; set; }        //Private set?

        public BookCover()
        {
            Id = Guid.NewGuid();
            TextId = Guid.NewGuid();
        }

    }
}