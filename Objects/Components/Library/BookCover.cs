namespace Objects.Components.Library
{
    public class BookCover
    {
        public Guid Id { get; private set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public BookFormat Format { get; set; }

        public BookCover()
        {
            Id = Guid.NewGuid();
        }

    }

    //public enum BookFormat
    //{
    //    pdf,
    //    epub
    //}
}