using Microsoft.EntityFrameworkCore;
using Objects.Entities;

namespace BlazorServer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var bookId = Guid.NewGuid();
            //Seed books

            modelBuilder.Entity<Book>(
                entity =>
                {
                    entity.HasOne(c => c.BookCover)
                        .WithOne(b => b.Book);
                });

            modelBuilder.Entity<Book>().HasData(new Book
            {
                Id = bookId,
                Text = "Text"
            });

            modelBuilder.Entity<BookCover>().HasData(new BookCover
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                Author = "Author",
                Title = "Title2",
                Description = "Description",
                Format = BookFormat.epub.ToString()
            });
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCover> BookCovers { get; set; }
    }
}
