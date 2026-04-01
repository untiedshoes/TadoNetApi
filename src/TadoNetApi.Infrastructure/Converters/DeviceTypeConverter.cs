using System.Text.Json;
using System.Text.Json.Serialization;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Converters
{
    /// <summary>
    /// Converts the Tado device type returned by the Tado API to the DeviceTypes enumerator in this project
    /// </summary>
    public class DeviceTypeConverter : JsonConverter<DeviceTypes?>
    {
        public override DeviceTypes? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                "HEATING" => DeviceTypes.Heating,
                "HOT_WATER" => DeviceTypes.HotWater,
                _ => null
            };
        }

        public override void Write(Utf8JsonWriter writer, DeviceTypes? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            switch (value)
            {
                case DeviceTypes.Heating:
                    writer.WriteStringValue("HEATING");
                    break;

                case DeviceTypes.HotWater:
                    writer.WriteStringValue("HOT_WATER");
                    break;

                default:
                    writer.WriteNullValue();
                    break;
            }
        }
    }
}