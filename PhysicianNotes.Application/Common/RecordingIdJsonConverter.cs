using PhysicianNotes.Domain.Recording;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhysicianNotes.Application.Common;

public class RecordingIdJsonConverter : JsonConverter<RecordingId>
{
    public override RecordingId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var guid = doc.RootElement.GetProperty("Value").GetGuid();
        return RecordingId.Of(guid);
    }

    public override void Write(Utf8JsonWriter writer, RecordingId value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Value", value.Value);
        writer.WriteEndObject();
    }
}
