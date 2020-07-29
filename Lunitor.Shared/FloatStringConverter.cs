using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lunitor.Shared
{
    public class FloatStringConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var valueString = reader.GetString();

                if (valueString.ToLower() == "infinity" || valueString == "∞")
                    return float.PositiveInfinity;
                else if(valueString.ToLower() == "-infinity" || valueString == "-∞")
                    return float.NegativeInfinity;

                return float.Parse(valueString);
            }

            return reader.GetSingle();
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                writer.WriteStringValue(value.ToString());
            }
            else
            {
                writer.WriteNumberValue(value);
            }
        }
    }
}
