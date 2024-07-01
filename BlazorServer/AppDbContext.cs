using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;
using Objects.Entities.Books;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Books.TxtBook;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using System.Net;

namespace BlazorServer
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookCover>(entity =>
            {
                entity.HasKey(bc => bc.Id);
                entity.HasOne<AbstractBook>()
                    .WithOne(ab => ab.BookCover)
                    .HasForeignKey<BookCover>(bc => bc.BookId);
            });

            modelBuilder.Entity<EpubBook>().ToTable("EpubBooks");       //Table-per-type configuration
            modelBuilder.Entity<PdfBook>().ToTable("PdfBooks");
            modelBuilder.Entity<TxtBook>().ToTable("TxtBooks");

            modelBuilder.Entity<EpubBook>()     //Book Id refers to Abstract book Id
                .Property(b => b.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<PdfBook>()
                .Property(b => b.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<TxtBook>()
                .Property(b => b.Id)
                .HasColumnName("Id");
        }

        public DbSet<AbstractBook> AbstractBooks { get; set; }
        public DbSet<EpubBook> EpubBooks { get; set; }
        public DbSet<PdfBook> PdfBooks { get; set; }
        public DbSet<TxtBook> TxtBooks { get; set; }
        public DbSet<BookCover> BookCovers { get; set; }
        public DbSet<BookSection> BookSections { get; set; }
        public DbSet<SavedWord> SavedWords { get; set; }
    }
}
