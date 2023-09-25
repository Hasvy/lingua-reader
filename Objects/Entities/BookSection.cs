using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities
{
    public class BookSection
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public int OrderNumber { get; set; }
        public IEnumerable<Page> Pages { get; set; }
        public string? Text { get; set; }

        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }
        public BookSection()
        {
            Id = Guid.NewGuid();
        }
    }
}
