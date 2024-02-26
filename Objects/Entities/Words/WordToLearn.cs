using Objects.Entities.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Words
{
    public class WordToLearn
    {
        public WordWithTranslations WordWithTranslations { get; set; } = new WordWithTranslations();
        public string[] WrongVariants { get; set; } = new string[2];
        public string Answer { get; set; } = string.Empty;
        public bool? UserRemember { get; set; } = null;
    }
}
