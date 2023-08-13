using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Services
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if(Guid.TryParse((string)reader.Value, out Guid result))
                {
                    return result;
                }
            }
            throw new JsonSerializationException("Invalid Guid format");
        }
        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
