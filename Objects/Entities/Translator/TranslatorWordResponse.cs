using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Translator
{
    public class TranslatorWordResponse
    {
        public string displaySource { get; set; } = null!;
        public IList<WordTranslation> translations { get; set; } = new List<WordTranslation>();
    }
}
