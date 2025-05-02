using System.Text.Json;
using RuanFa.Shop.Application.Common.Extensions.Converters;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;

public static class FilterCriteriaExtensions
{
    public static IReadOnlyList<FilterCriteria> ToFilterCriteria(this string? filters)
    {
        if (string.IsNullOrEmpty(filters))
            return Array.Empty<FilterCriteria>();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new FilterOperatorConverter() }
            };

            return JsonSerializer.Deserialize<List<FilterCriteria>>(filters, options) ?? [];
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid filters parameter: {ex.Message}", nameof(filters), ex);
        }
    }
}
