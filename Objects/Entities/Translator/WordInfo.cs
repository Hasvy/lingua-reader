using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Translator
{
    public class WordInfo
    {
        public WordInfo() { }
        public string Word { get; set; } = string.Empty;
        public float Height { get; set; }       //Mb delete
        public float Width { get; set; }        //del
        public float Top { get; set; }
        public float Left { get; set; }
    }
}
