using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationService.CustomJsonConverter
{
    public class StringEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string enumValueString = reader.GetString();
            if (Enum.TryParse(enumValueString, true, out TEnum enumValue))
            {
                return enumValue;
            }

            throw new JsonException($"Unable to parse enum value: {enumValueString}");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
