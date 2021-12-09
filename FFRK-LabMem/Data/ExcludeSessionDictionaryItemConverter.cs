using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class ExcludeSessionDictionaryItemConverter<TDictionary, TValue> : JsonConverter where TDictionary : IDictionary<string, TValue>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TDictionary).IsAssignableFrom(objectType);
        }

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = (JObject)t;
                o.Remove("Session");
                o.WriteTo(writer);
            }
        }
    }

}
