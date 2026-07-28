using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhysicianNotes.Application.Common;

public class StronglyTypedGuidIdConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.GetProperty("Value")?.PropertyType == typeof(Guid)
           && typeToConvert.GetMethod("Of", new[] { typeof(Guid) }) != null;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(StronglyTypedGuidIdConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class StronglyTypedGuidIdConverter<T> : JsonConverter<T>
{
    private static readonly MethodInfo OfMethod = typeof(T).GetMethod("Of", new[] { typeof(Guid) })!;

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Guid guid = reader.TokenType == JsonTokenType.String
            ? reader.GetGuid()
            : JsonDocument.ParseValue(ref reader).RootElement.GetProperty("Value").GetGuid();

        return (T)OfMethod.Invoke(null, new object[] { guid })!;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var guidValue = (Guid)typeof(T).GetProperty("Value")!.GetValue(value)!;
        writer.WriteStartObject();
        writer.WriteString("Value", guidValue);
        writer.WriteEndObject();
    }
}
