using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public virtual BookCover BookCover { get; set; }
        public string? Text { get; set; }
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

    public enum BookFormat
    {
        pdf,
        epub
    }
}
