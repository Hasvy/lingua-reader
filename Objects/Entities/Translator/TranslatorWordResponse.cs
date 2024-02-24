using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Translator
{
    public class TranslatorWordResponse
    {
        public int Id { get; set; }
        public string DisplaySource { get; set; } = null!;
        public string Language { get; set; } = null!;
        public IList<WordTranslation> Translations { get; set; } = new List<WordTranslation>();
        [NotMapped]
        public bool IsWordSaved { get; set; } = false;
    }
}
