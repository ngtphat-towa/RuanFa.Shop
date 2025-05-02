using System.Text.Json;

namespace RuanFa.Shop.Application.Common.Extensions.Converters;

public static class FilterValueConverter
{
    public static object? Convert(object? value, Type targetType)
    {
        if (value == null) return null;

        try
        {
            if (targetType.IsEnum)
                return value is string str ? Enum.Parse(targetType, str, true) : Enum.ToObject(targetType, value);

            if (value is JsonElement jsonElement)
                return jsonElement.Deserialize(targetType);

            if (targetType == typeof(Guid) && value is string guid)
                return Guid.Parse(guid);

            return targetType == typeof(DateTime ) && value is string dt
                ? DateTime .Parse(dt)
                : System.Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to convert value '{value}' to type {targetType.Name}", ex);
        }
    }
}
