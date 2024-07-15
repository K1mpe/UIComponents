using System.Text.Json;
using System.Text.Json.Serialization;

namespace UIComponents.Models.Converters
{
    public class TranslatableConverter : JsonConverter<Translatable>
    {
        public override Translatable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (Translatable)reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, Translatable value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Serialize());
        }
    }
}
