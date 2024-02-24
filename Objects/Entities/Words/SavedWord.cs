using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Words
{
    public class SavedWord
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int WordId { get; set; }
    }
}
