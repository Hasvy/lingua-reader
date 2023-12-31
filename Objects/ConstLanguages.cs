﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
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

        public static readonly string[] BookLanguagesSet = new string[] { English, German, Russian, Czech, Italian, Spanish };
        public static readonly string[] TargetLanguagesSet = new string[] { English, German, Russian, Czech, Italian, Spanish, Ukrainian, French };
    }
}
