using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Books
{
    public abstract class AbstractBook      //If future I can move all properties from BookCover to AbstractBook object/table
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public virtual BookCover BookCover { get; set; } = null!;

        public AbstractBook()
        {
            Id = Guid.NewGuid();
        }
    }
}
