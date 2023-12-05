using Microsoft.EntityFrameworkCore;
using Objects.Entities.Translator;

namespace BlazorServer
{
    public class DictionaryDbContext : DbContext
    {
        public DictionaryDbContext(DbContextOptions<DictionaryDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TranslatorWordResponse>(entity =>
            {
                entity.HasKey(twr => twr.Id);
                entity.HasMany(wt => wt.Translations)
                .WithOne()
                .HasForeignKey(wt => wt.WordId);
            });

            modelBuilder.Entity<WordTranslation>(entity =>
            {
                entity.HasKey(wt => wt.Id);
            });

            //modelBuilder.Entity<TranslationEnglish>()
            //   .Property(te => te.Id)
            //   .HasColumnName("Id");

            //modelBuilder.Entity<TranslationGerman>()
            //   .Property(te => te.Id)
            //   .HasColumnName("Id");

            //modelBuilder.Entity<TranslationRussian>()
            //   .Property(te => te.Id)
            //   .HasColumnName("Id");

            //modelBuilder.Entity<TranslationEnglish>().UseTptMappingStrategy().ToTable("Translations");
            //modelBuilder.Entity<TranslationEnglish>().ToTable("TranslationsEnglish");
            //modelBuilder.Entity<TranslationGerman>().ToTable("TranslationsGerman");
            //modelBuilder.Entity<TranslationRussian>().ToTable("TranslationsRussian");
        }

        public DbSet<TranslatorWordResponse> Words { get; set; }
        public DbSet<WordTranslation> Translations { get; set; }
        //public DbSet<TranslationEnglish> TranslationsEnglish { get; set; }
        //public DbSet<TranslationGerman> TranslationsGerman { get; set; }
        //public DbSet<TranslationRussian> TranslationsRussian { get; set; }
    }
}
