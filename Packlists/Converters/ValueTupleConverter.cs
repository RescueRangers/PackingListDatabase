using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Packlists.Converters
{
    internal class ValueTupleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = null;
            var dict = new Dictionary<Tuple<int, int>, object>();

            //loop through the JSON string reader
            while (reader.Read())
            {
                // check whether it is a property
                if (reader.TokenType != JsonToken.PropertyName) continue;
                
                var readerValue = reader.Value.ToString();
                
                if (!reader.Read()) continue;
                // check if the property is tuple (Dictionary key)
                if (readerValue.Contains('(') && readerValue.Contains(')'))
                {
                    var result = readerValue.Split(new[] { '(', ',', ')'},
                        StringSplitOptions.RemoveEmptyEntries);

                    // Custom Deserialize the Dictionary key (Tuple)
                    var tuple = Tuple.Create<int, int>(int.Parse(result[0].Trim()), int.Parse(result[1].Trim()));

                    // Custom Deserialize the Dictionary value
                    value = serializer.Deserialize(reader);

                    dict.Add(tuple, value);
                }
                else
                {
                    // Deserialize the remaining data from the reader
                    serializer.Deserialize(reader);
                    break;
                }
            }
            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override bool CanRead => true;
    }
}
