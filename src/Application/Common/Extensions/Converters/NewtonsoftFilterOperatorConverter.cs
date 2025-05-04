using Newtonsoft.Json;
using RuanFa.Shop.SharedKernel.Enums;

namespace RuanFa.Shop.Application.Common.Extensions.Converters;

public class NewtonsoftFilterOperatorConverter : JsonConverter<FilterOperator>
{
    public override FilterOperator ReadJson(
        JsonReader reader, 
        Type objectType, FilterOperator existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string? value = reader.Value?.ToString();
        if (string.IsNullOrEmpty(value) || !Enum.TryParse<FilterOperator>(value, true, out var result))
        {
            throw new JsonSerializationException($"Invalid FilterOperator value: {value}");
        }
        return result;
    }

    public override void WriteJson(JsonWriter writer, FilterOperator value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
