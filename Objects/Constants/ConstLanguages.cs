using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Constants
{
    //public class Language     TODO change to object
    //{
    //    public readonly string ShortLang;
    //    public readonly string LongLang;

    //    public Language(string shortLang, string longLang)
    //    {
    //        ShortLang = shortLang;
    //        LongLang = longLang;
    //    }
    //}

    public class ConstLanguages
    {
        public const string English = "en";
        public const string German = "de";
        public const string Russian = "ru";
        public const string Czech = "cs";
        public const string Italian = "it";
        public const string Spanish = "es";
        public const string Ukrainian = "uk";
        public const string French = "fr";
        public const string Undefined = "undefined";

        public const string EnglishFull = "English";
        public const string GermanFull = "Deutsch";
        public const string RussianFull = "Русский";
        public const string CzechFull = "Čeština";
        public const string ItalianFull = "Italiano";
        public const string SpanishFull = "Español";
        public const string UkrainianFull = "Українська";
        public const string FrenchFull = "Français";

        public static readonly string[] BookLanguagesSet = new string[] { English, German, Russian, Czech, Italian, Spanish };
        public static readonly string[] TargetLanguagesSet = new string[] { English, German, Russian, Czech, Italian, Spanish, Ukrainian, French };

        public static string GetFlagPath(string language)
        {
            switch (language)
            {
                case Czech: return "img/czech-republic-32.png";
                case English: return "img/great-britain-32.png";
                case German: return "img/germany-32.png";
                case Russian: return "img/russian-federation-32.png";
                case Italian: return "img/italy-32.png";
                case Spanish: return "img/spain-flag-32.png";
                case Ukrainian: return "img/ukraine-flag-icon-32.png";
                case French: return "img/france-flag-icon-32.png";
                default: return "img/question-32.png";
            }
        }

        public static string GetFullLang(string language)
        {
            switch (language)
            {
                case Czech: return CzechFull;
                case English: return EnglishFull;
                case German: return GermanFull;
                case Russian: return RussianFull;
                case Italian: return ItalianFull;
                case Spanish: return SpanishFull;
                case Ukrainian: return UkrainianFull;
                case French: return FrenchFull;
                default: return Undefined;
            }
        }
    }
}
