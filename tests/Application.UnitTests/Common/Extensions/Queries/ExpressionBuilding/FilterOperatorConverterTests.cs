using System.Text.Json;
using RuanFa.Shop.Application.Common.Extensions.Converters;
using RuanFa.Shop.SharedKernel.Enums;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries.ExpressionBuilding;

public class FilterOperatorConverterTests
{
    public static TheoryData<string, FilterOperator> ValidStringOperators =>
        new()
        {
            { "\"Equals\"", FilterOperator.Equals },
            { "\"equals\"", FilterOperator.Equals },
            { "\"EQUALS\"", FilterOperator.Equals },
            { "\"Equal\"", FilterOperator.Equal },
            { "\"Contains\"", FilterOperator.Contains },
            { "\"Contain\"", FilterOperator.Contain },
            { "\"StartsWith\"", FilterOperator.StartsWith },
            { "\"EndsWith\"", FilterOperator.EndsWith },
            { "\"GreaterThan\"", FilterOperator.GreaterThan },
            { "\"GreaterThanOrEqual\"", FilterOperator.GreaterThanOrEqual },
            { "\"LessThan\"", FilterOperator.LessThan },
            { "\"LessThanOrEqual\"", FilterOperator.LessThanOrEqual },
            { "\"In\"", FilterOperator.In },
            { "\"NotIn\"", FilterOperator.NotIn },
            { "\"IsNull\"", FilterOperator.IsNull },
            { "\"IsNotNull\"", FilterOperator.IsNotNull },
            { "\"Range\"", FilterOperator.Range }
        };

    public static TheoryData<string, FilterOperator> ValidNumericOperators =>
        new()
        {
            { "0", FilterOperator.Equal },
            { "1", FilterOperator.Equals },
            { "2", FilterOperator.NotEqual },
            { "3", FilterOperator.NotEquals },
            { "4", FilterOperator.Contain },
            { "5", FilterOperator.Contains },
            { "8", FilterOperator.GreaterThan },
            { "9", FilterOperator.GreaterThanOrEqual },
            { "16", FilterOperator.Range }
        };

    public static TheoryData<string, string> InvalidOperators =>
        new()
        {
            { "\"InvalidOperator\"", "Unable to convert value 'InvalidOperator' to FilterOperator." },
            { "\"NotExists\"", "Unable to convert value 'NotExists' to FilterOperator." },
            { "\"Unknown\"", "Unable to convert value 'Unknown' to FilterOperator." },
            { "\"999\"", "Value '999' is not a valid FilterOperator." },
            { "\"\"", "Unable to convert empty value to FilterOperator." },
            { "null", "Unexpected token type when converting to FilterOperator: Null." },
            { "{}", "Unexpected token type when converting to FilterOperator: StartObject." },
            { "[]", "Unexpected token type when converting to FilterOperator: StartArray." },
            { "true", "Unexpected token type when converting to FilterOperator: True." },
            { "\"Contains_\"", "Unable to convert value 'Contains_' to FilterOperator." },
            { "\"_Contains\"", "Unable to convert value '_Contains' to FilterOperator." }
        };

    [Theory]
    [MemberData(nameof(ValidStringOperators))]
    public void Deserialize_WithValidStringOperator_ReturnsExpectedEnum(string json, FilterOperator expected)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() }
        };

        var result = JsonSerializer.Deserialize<FilterOperator>(json, options);

        result.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(ValidNumericOperators))]
    public void Deserialize_WithValidNumericOperator_ReturnsExpectedEnum(string json, FilterOperator expected)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() }
        };

        var result = JsonSerializer.Deserialize<FilterOperator>(json, options);

        result.ShouldBe(expected);
    }


    [Theory]
    [MemberData(nameof(InvalidOperators))]
    public void Deserialize_WithInvalidValue_ThrowsJsonException(string json, string expectedMessage)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() }
        };

        var ex = Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<FilterOperator>(json, options));

        ex.Message.ShouldBe(expectedMessage);
    }

    [Fact]
    public void Serialize_AllEnumValues_SerializesCorrectly()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() }
        };

        foreach (FilterOperator op in Enum.GetValues(typeof(FilterOperator)))
        {
            var json = JsonSerializer.Serialize(op, options);
            var deserializedOp = JsonSerializer.Deserialize<FilterOperator>(json, options);

            deserializedOp.ShouldBe(op);
        }
    }

    [Fact]
    public void Deserialize_WithCaseInsensitiveOptions_HandlesAllCases()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() },
            PropertyNameCaseInsensitive = true
        };

        var variations = new[] { "\"equals\"", "\"EQUALS\"", "\"Equals\"", "\"eQuAlS\"" };

        foreach (var json in variations)
        {
            var result = JsonSerializer.Deserialize<FilterOperator>(json, options);
            result.ShouldBe(FilterOperator.Equals);
        }
    }

    [Fact]
    public void Deserialize_WithWhitespace_HandlesCorrectly()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterOperatorConverter() }
        };

        var json = "\" Equals \""; // With whitespace

        var result = JsonSerializer.Deserialize<FilterOperator>(json, options);
        result.ShouldBe(FilterOperator.Equals);
    }
}
