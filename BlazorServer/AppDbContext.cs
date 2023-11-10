using Microsoft.EntityFrameworkCore;
using Objects.Entities;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;
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

            //modelBuilder.Entity<AbstractBook>()
            //    .HasOne(b => b.BookCover)
            //    .WithOne(bc => bc.book);

            modelBuilder.Entity<AbstractBook>(entity =>
            {
                entity.HasKey(ab => ab.Id);
            });

            modelBuilder.Entity<BookCover>(entity =>
            {
                entity.HasKey(bc => bc.Id);
                entity.HasOne<AbstractBook>()
                    .WithOne(ab => ab.BookCover)
                    .HasForeignKey<BookCover>(bc => bc.BookId);
            });

            modelBuilder.Entity<EpubBook>().ToTable("EpubBooks");       //Table-per-type configuration

            modelBuilder.Entity<PdfBook>().ToTable("PdfBooks");

            modelBuilder.Entity<EpubBook>()     //Epub book column Id refers to Abstract book Id column
                .Property(b => b.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<PdfBook>()
                .Property(b => b.Id)
                .HasColumnName("Id");

            //modelBuilder.Entity<BookSection>()
            //    .HasOne(s => s.EpubBook)
            //    .WithMany(b => b.Sections)
            //    .HasForeignKey(s => s.EpubBook);

            //modelBuilder.Entity<EpubBook>()
            //    .HasMany(b => b.Sections)
            //    .WithOne(s => s.EpubBook)
            //    .HasForeignKey(s => s.EpubBookId);

            //modelBuilder.Entity<EpubBook>()
            //    .HasMany(b => b.Content)
            //    .WithOne(c => c.EpubBook)
            //    .HasForeignKey(s => s.BookId);

            //modelBuilder.Entity<BookSection>()
            //    .HasMany(s => s.Pages)
            //    .WithOne(p => p.Section)
            //    .HasForeignKey(p => p.SectionId)
            //    .HasPrincipalKey(s => s.Id);
        }

        public DbSet<AbstractBook> AbstractBooks { get; set; }
        public DbSet<EpubBook> EpubBooks { get; set; }
        public DbSet<PdfBook> PdfBooks { get; set; }
        public DbSet<BookCover> BookCovers { get; set; }
        public DbSet<BookSection> BookSections { get; set; }
        //public DbSet<Page> Pages { get; set; }
        //public DbSet<BookContent> BookContent { get; set; }
    }
}
