using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Words
{
    public class EnglishWord
    {
        public int Id { get; set; }
        public string OriginalWord { get; set; } = null!;
        public string SimilarWords { get; set; }    //Maybe not
        public string RussianTranslations { get; set; }
        public string CzechTranslations { get; set; }
        public string GermanTranslations { get; set; }
        public string ItalianTranslations { get; set; }
        public string SpanishTranslations { get; set; }
        public string UkrainianTranslations { get; set; }
        public string FrenchTranslations { get; set; }
        public string Transcription { get; set; }
        public string Examples { get; set; }
    }
}
