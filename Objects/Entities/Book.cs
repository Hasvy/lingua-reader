using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public virtual BookCover BookCover { get; set; }
        //public string? Text { get; set; }
        public virtual IEnumerable<BookSection> Sections { get; set; }
        public virtual IEnumerable<BookContent> Content { get; set; }
        public int PagesCount { get; set; }
        public int SectionsCount { get; set; }
        [NotMapped]
        public string BookContentFile { get; set; }
        //public List<string> ContentFiles { get; set; }

        //public Book()
        //{
        //    BookCover = new BookCover();
        //}
        public Book()
        {
            Id = Guid.NewGuid();
        }
    }
}
