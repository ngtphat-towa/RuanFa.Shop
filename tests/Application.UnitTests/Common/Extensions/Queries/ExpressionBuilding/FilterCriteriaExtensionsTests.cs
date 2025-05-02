using System.Text.Json;
using RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;
using RuanFa.Shop.SharedKernel.Enums;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries.ExpressionBuilding;

public class FilterCriteriaExtensionsTests
{
    public static TheoryData<string?, IReadOnlyList<FilterCriteria>> ValidFilterData =>
        new()
        {
            {
                null,
                Array.Empty<FilterCriteria>()
            },
            {
                "",
                Array.Empty<FilterCriteria>()
            },
            {
                JsonSerializer.Serialize(new[]
                {
                    new FilterCriteria { Field = "Id", Operator = FilterOperator.GreaterThan, Value = 10 }
                }),
                new[]
                {
                    new FilterCriteria { Field = "Id", Operator = FilterOperator.GreaterThan, Value = 10 }
                }
            },
            {
                JsonSerializer.Serialize(new[]
                {
                    new FilterCriteria { Field = "Name", Operator = FilterOperator.Contains, Value = "foo" },
                    new FilterCriteria { Field = "Active", Operator = FilterOperator.Equals, Value = true }
                }),
                new[]
                {
                    new FilterCriteria { Field = "Name", Operator = FilterOperator.Contains, Value = "foo" },
                    new FilterCriteria { Field = "Active", Operator = FilterOperator.Equals, Value = true }
                }
            }
        };

    public static TheoryData<string, FilterOperator> OperatorCaseData =>
        new()
        {
            { "equals", FilterOperator.Equals },
            { "EQUALS", FilterOperator.Equals },
            { "eQuAls", FilterOperator.Equals },
            { "contains", FilterOperator.Contains },
            { "CONTAINS", FilterOperator.Contains }
        };

    public static TheoryData<string> InvalidJsonData =>
        new()
        {
            "{not valid json}",
            "[{incomplete",
            "{'malformed': true",
            "[{ \"Field\":\"X\",\"Operator\":\"InvalidOperator\",\"Value\":5 }]"
        };


    [Theory]
    [MemberData(nameof(ValidFilterData))]
    public void ToFilterCriteria_ValidInput_ReturnsExpectedResult(string? input, IReadOnlyList<FilterCriteria> expected)
    {
        var result = input.ToFilterCriteria();

        result.Count.ShouldBe(expected.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            result[i].Field.ShouldBe(expected[i].Field);
            result[i].Operator.ShouldBe(expected[i].Operator);
            CompareValues(result[i].Value, expected[i].Value);
        }
    }

    [Theory]
    [MemberData(nameof(InvalidJsonData))]
    public void ToFilterCriteria_InvalidJson_ThrowsArgumentException(string invalidJson)
    {
        var ex = Should.Throw<ArgumentException>(() => invalidJson.ToFilterCriteria());
        ex.Message.ShouldContain("Invalid filters parameter");
        ex.ParamName.ShouldBe("filters");
    }

    [Theory]
    [MemberData(nameof(OperatorCaseData))]
    public void ToFilterCriteria_OperatorCaseInsensitive_Works(string operatorText, FilterOperator expectedOperator)
    {
        var json = $"[{{ \"Field\":\"X\",\"Operator\":\"{operatorText}\",\"Value\":5 }}]";
        var list = json.ToFilterCriteria();

        list[0].Operator.ShouldBe(expectedOperator);
    }

    [Fact]
    public void ToFilterCriteria_DifferentValueTypes_ParsesCorrectly()
    {
        var json = JsonSerializer.Serialize(new[]
        {
            new FilterCriteria { Field = "StringField", Operator = FilterOperator.Equals, Value = "text" },
            new FilterCriteria { Field = "IntField", Operator = FilterOperator.Equals, Value = 42 },
            new FilterCriteria { Field = "BoolField", Operator = FilterOperator.Equals, Value = true },
            new FilterCriteria { Field = "DecimalField", Operator = FilterOperator.Equals, Value = 12.34m }
        });

        var result = json.ToFilterCriteria();

        CompareValues(result[0].Value, "text");
        CompareValues(result[1].Value, 42);
        CompareValues(result[2].Value, true);
        CompareValues(result[3].Value, 12.34m);
    }

    private static void CompareValues(object? actual, object? expected)
    {
        if (expected == null)
        {
            actual.ShouldBeNull();
            return;
        }

        actual.ShouldNotBeNull();

        // Handle JsonElement conversion
        if (actual is JsonElement jsonElement)
        {
            switch (expected)
            {
                case string expectedString:
                    jsonElement.GetString().ShouldBe(expectedString);
                    break;
                case int expectedInt:
                    jsonElement.GetInt32().ShouldBe(expectedInt);
                    break;
                case bool expectedBool:
                    jsonElement.GetBoolean().ShouldBe(expectedBool);
                    break;
                case decimal expectedDecimal:
                    jsonElement.GetDecimal().ShouldBe(expectedDecimal);
                    break;
                case double expectedDouble:
                    jsonElement.GetDouble().ShouldBe(expectedDouble);
                    break;
                default:
                    actual.ToString().ShouldBe(expected.ToString());
                    break;
            }
            return;
        }

        // Handle non-JsonElement comparisons
        switch (expected)
        {
            case string expectedString:
                actual.ToString().ShouldBe(expectedString);
                break;
            case int expectedInt:
                actual.ShouldBeOfType<int>();
                actual.ShouldBe(expectedInt);
                break;
            case bool expectedBool:
                actual.ShouldBeOfType<bool>();
                actual.ShouldBe(expectedBool);
                break;
            case decimal expectedDecimal:
                actual.ShouldBeOfType<decimal>();
                actual.ShouldBe(expectedDecimal);
                break;
            default:
                actual.ShouldBe(expected);
                break;
        }
    }

}
