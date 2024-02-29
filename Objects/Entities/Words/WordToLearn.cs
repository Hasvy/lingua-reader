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
        public WordWithTranslations WordWithTranslations { get; set; } = new WordWithTranslations();    //TODO Maybe change to only one translation and word in main language properties
        public WordTranslation[] WrongVariants { get; set; } = new WordTranslation[3];
        public List<VariantToAnswer> VariantsToAnswer { get; set; } = new List<VariantToAnswer>();
        public VariantToAnswer? Answer { get; set; }
        public bool? UserRemember { get; set; } = null;
    }
}
