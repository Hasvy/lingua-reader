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

            modelBuilder.Entity<WordWithTranslations>(entity =>
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
        }

        public DbSet<WordWithTranslations> Words { get; set; }
        public DbSet<WordTranslation> Translations { get; set; }
    }
}
