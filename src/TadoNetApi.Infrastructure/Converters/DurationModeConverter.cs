using System.Text.Json;
using System.Text.Json.Serialization;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Converters
{
    /// <summary>
    /// Converts the duration mode type returned by the Tado API to the DurationModes enumerator in this project
    /// </summary>
    public class DurationModeConverter : JsonConverter<DurationModes?>
    {
        public override DurationModes? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                return null;
            }

            var enumString = reader.GetString();
            if (string.IsNullOrEmpty(enumString))
            {
                return null;
            }

            return enumString switch
            {
                "MANUAL" => DurationModes.UntilNextManualChange,
                "TADO_MODE" => DurationModes.UntilNextTimedEvent,
                "TIMER" => DurationModes.Timer,
                _ => null
            };
        }

        public override void Write(Utf8JsonWriter writer, DurationModes? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            switch (value)
            {
                case DurationModes.UntilNextManualChange:
                    writer.WriteStringValue("MANUAL");
                    break;

                case DurationModes.UntilNextTimedEvent:
                    writer.WriteStringValue("TADO_MODE");
                    break;

                case DurationModes.Timer:
                    writer.WriteStringValue("TIMER");
                    break;

                default:
                    writer.WriteNullValue();
                    break;
            }
        }
    }
}