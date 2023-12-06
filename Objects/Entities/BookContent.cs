using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objects.Entities.Books.EpubBook;

namespace Objects.Entities
{
    public class BookContent
    {
        public Guid Id { get; private set; }
        public string Type { get; set; }
        public string Content { get; set; }

        public Guid BookId { get; set; }
        public virtual EpubBook? Book { get; set; }
        public BookContent() 
        {
            Id = Guid.NewGuid();
        }
    }

    public class ContentType
    {
        public const string css = "css";
        public const string image = "image";
    }
}
