using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartBinSensor.SensorMessage;

internal class SensorMessageJsonConverter : JsonConverter<SensorMessage>
{
    public override SensorMessage? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // not needed for now
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        SensorMessage value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteString("sensor-id", value.Id.Value.ToString());
        writer.WriteString("timestamp", value.Timestamp.ToString("o"));
        writer.WriteNumber("fill-level", value.FillLevel);
        writer.WriteEndObject();
    }
}
