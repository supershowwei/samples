using System;
using Newtonsoft.Json;
using SamplesForm.Model.Parameters;

namespace SamplesForm.JsonConverters
{
    public class FieldJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Activator.CreateInstance(objectType, reader.Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Field<>);
        }
    }
}