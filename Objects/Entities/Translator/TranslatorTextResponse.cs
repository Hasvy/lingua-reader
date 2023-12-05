using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Entities.Translator
{
    public class TranslatorTextResponse
    {
        [JsonProperty("translations")]
        public List<TranslationItem> translations { get; set; } = null!;
    }

    public class TranslationItem
    {
        [JsonProperty("text")]
        public string text { get; set; } = null!;
        [JsonProperty("to")]
        public string to { get; set; } = null!;
    }
}
