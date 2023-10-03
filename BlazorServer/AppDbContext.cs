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

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Sections)
                .WithOne(s => s.Book)
                .HasForeignKey(s => s.BookId);

            //modelBuilder.Entity<Book>()
            //    .HasMany(b => b.Content)
            //    .WithOne(c => c.Book)
            //    .HasForeignKey(s => s.BookId);

            modelBuilder.Entity<BookSection>()
                .HasMany(s => s.Pages)
                .WithOne(p => p.Section)
                .HasForeignKey(p => p.SectionId)
                .HasPrincipalKey(s => s.Id);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCover> BookCovers { get; set; }
        public DbSet<BookSection> BookSections { get; set; }
        public DbSet<Page> Pages { get; set; }
        //public DbSet<BookContent> BookContent { get; set; }
    }
}
