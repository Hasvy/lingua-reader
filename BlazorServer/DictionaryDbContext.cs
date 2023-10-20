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
            modelBuilder.Entity<WordTranslation>(entity =>
            {
                entity.HasKey(w => w.Id);
            });
        }

        public DbSet<WordTranslation> WordTranslations { get; set; }
    }
}
