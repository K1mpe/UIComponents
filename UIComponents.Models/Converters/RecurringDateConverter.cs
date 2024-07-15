using System.Text.Json;
using System.Text.Json.Serialization;
using UIComponents.Abstractions.DataTypes.RecurringDates;

namespace UIComponents.Models.Converters;

public class RecurringDateConverter : JsonConverter<RecurringDate>
{
    public override RecurringDate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return RecurringDate.Deserialize(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, RecurringDate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Serialize());
    }
}
