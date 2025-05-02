using System.Text.Json;
using System.Text.Json.Serialization;
using RuanFa.Shop.SharedKernel.Enums;

namespace RuanFa.Shop.Application.Common.Extensions.Converters;

public class FilterOperatorConverter : JsonConverter<FilterOperator>
{
    public override FilterOperator Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                    throw new JsonException("Unable to convert empty value to FilterOperator.");

                // First, handle numeric strings
                if (stringValue.All(char.IsDigit))
                {
                    if (int.TryParse(stringValue, out var numericValue) &&
                        !Enum.IsDefined(typeof(FilterOperator), numericValue))
                    {
                        throw new JsonException($"Value '{stringValue}' is not a valid FilterOperator.");
                    }
                }

                // Then try parsing as enum string
                if (Enum.TryParse<FilterOperator>(stringValue, ignoreCase: true, out var enumResult))
                {
                    return enumResult;
                }

                throw new JsonException($"Unable to convert value '{stringValue}' to FilterOperator.");

            case JsonTokenType.Number:
                if (!reader.TryGetInt32(out var intValue))
                    throw new JsonException("Unable to convert numeric value to FilterOperator.");

                if (!Enum.IsDefined(typeof(FilterOperator), intValue))
                    throw new JsonException($"Value '{intValue}' is not a valid FilterOperator.");

                return (FilterOperator)intValue;

            case JsonTokenType.Null:
                throw new JsonException("Unexpected token type when converting to FilterOperator: Null.");

            case JsonTokenType.StartObject:
                throw new JsonException("Unexpected token type when converting to FilterOperator: StartObject.");

            case JsonTokenType.StartArray:
                throw new JsonException("Unexpected token type when converting to FilterOperator: StartArray.");

            case JsonTokenType.True:
            case JsonTokenType.False:
                throw new JsonException("Unexpected token type when converting to FilterOperator: True.");

            default:
                throw new JsonException($"Unexpected token type when converting to FilterOperator: {reader.TokenType}.");
        }
    }

    public override void Write(Utf8JsonWriter writer, FilterOperator value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
