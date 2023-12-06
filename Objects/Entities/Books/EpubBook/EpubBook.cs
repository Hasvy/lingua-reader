using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Books.EpubBook
{
    public class EpubBook : AbstractBook
    {
        public virtual IList<BookSection> Sections { get; set; } = null!;
        public int SectionsCount { get; set; }
        [NotMapped]
        public string BookContentFile { get; set; } = null!;
    }
}
