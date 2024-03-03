using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Books.TxtBook
{
    public class TxtBook : AbstractBook
    {
        public string Text { get; set; } = null!;
    }
}
