using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Books.EpubBook
{
    public class BookSection
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public int OrderNumber { get; set; }
        public string? Text { get; set; }
        public Guid EpubBookId { get; set; }        //Foreign key
        [NotMapped]
        public int PagesCount { get; set; }
        [NotMapped]
        public int FirstPage { get; set; }
        [NotMapped]
        public int LastPage { get; set; }

        public BookSection()
        {
            Id = Guid.NewGuid();
        }
    }
}
