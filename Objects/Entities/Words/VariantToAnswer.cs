using Objects.Entities.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Words
{
    public class VariantToAnswer
    {
        public WordTranslation Text { get; set; } = null!;
        public bool isRight { get; set; } = false;

        public VariantToAnswer(WordTranslation wordTranslation, bool isRight = false)
        {
            Text = wordTranslation;
            this.isRight = isRight;
        }
    }
}
