using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Objects.Entities.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Objects.EntitiesConfigurations
{
    public class AbstractBookConfiguration : IEntityTypeConfiguration<AbstractBook>
    {
        public void Configure(EntityTypeBuilder<AbstractBook> builder)
        {
            builder.HasKey(ab => ab.Id);
        }
    }
}
