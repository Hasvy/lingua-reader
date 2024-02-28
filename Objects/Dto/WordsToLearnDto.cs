using Objects.Entities.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Dto
{
    public class WordsToLearnDto
    {
        public int WordsCount { get; set; }
        public List<WordToLearn>? WordsToLearn { get; set; }
    }
}
