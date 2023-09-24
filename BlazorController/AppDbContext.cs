using Microsoft.EntityFrameworkCore;
using Objects.Entities;
using System.Net;

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

            modelBuilder.Entity<Book>()
                .HasOne(b => b.BookCover)
                .WithOne(bc => bc.Book)
                .HasForeignKey<BookCover>(bc => bc.BookId);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCover> BookCovers { get; set; }
    }
}
