using System.Text.Json;
using System.Text.Json.Serialization;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Converters
{
    /// <summary>
    /// Converts the power state returned by the Tado API to the PowerStates enumerator in this project
    /// </summary>
    public class PowerStatesConverter : JsonConverter<PowerStates?>
    {
        public override PowerStates? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                "ON" => PowerStates.On,
                "OFF" => PowerStates.Off,
                _ => null
            };
        }

        public override void Write(Utf8JsonWriter writer, PowerStates? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            switch (value)
            {
                case PowerStates.On:
                    writer.WriteStringValue("ON");
                    break;

                case PowerStates.Off:
                    writer.WriteStringValue("OFF");
                    break;

                default:
                    writer.WriteNullValue();
                    break;
            }
        }
    }
}