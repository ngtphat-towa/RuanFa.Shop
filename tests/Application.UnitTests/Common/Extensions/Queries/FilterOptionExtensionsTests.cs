using System.Text.Json;
using RuanFa.Shop.Application.Common.Extensions.Converters;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.SharedKernel.Enums;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

public class FilterOptionExtensionsTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new FilterOperatorConverter() }
    };

    private static string SerializeFilters(FilterCriteria filter)
        => JsonSerializer.Serialize(new[] { filter }, SerializerOptions);

    private static string SerializeFilters(FilterCriteria[] filters)
        => JsonSerializer.Serialize(filters, SerializerOptions);

    private static IQueryable<TestEntity> TestData => new[]
    {
    new TestEntity
    {
        Id = 1,
        Name = "Alice",
        Age = 25,
        IsActive = true,
        Score = 10.5m,
        Tags = [ "A1", "A2" ],
        CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0)
    },
    new TestEntity
    {
        Id = 2,
        Name = "Bob",
        Age = 30,
        IsActive = false,
        Score = 20.7m,
        Tags = ["B1" ],
        CreatedDate = new DateTime(2025, 2, 1, 0, 0, 0)
    },
    new TestEntity
    {
        Id = 3,
        Name = "Carol",
        Age = 35,
        IsActive = true,
        Score = 30.2m,
        Tags = null,
        CreatedDate = new DateTime(2025, 3, 1, 0, 0, 0)
    },
    new TestEntity
    {
        Id = 4,
        Name = "Dave",
        Age = 40,
        IsActive = true,
        Score = 40.9m,
        Tags = [],
        CreatedDate = new DateTime(2025, 4, 1, 0, 0, 0)
    }
}.AsQueryable();


    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public decimal Score { get; set; }
        public string[]? Tags { get; set; }
        public DateTime  CreatedDate { get; set; }
    }

    #region Valid Filter Tests
    public class ValidFilterTests
    {
        public static TheoryData<string?, int> NullOrEmptyFiltersData => new()
        {
            { null, 4 },
            { "", 4 },
            { "[]", 4 }
        };

        public static TheoryData<FilterOperator, object, int> ComparisonOperatorsData => new()
        {
            { FilterOperator.Equal, 30, 1 },
            { FilterOperator.Equals, 30, 1 },
            { FilterOperator.NotEqual, 30, 3 },
            { FilterOperator.NotEquals, 30, 3 },
            { FilterOperator.GreaterThan, 30, 2 },
            { FilterOperator.GreaterThanOrEqual, 30, 3 },
            { FilterOperator.LessThan, 30, 1 },
            { FilterOperator.LessThanOrEqual, 30, 2 }
        };

        public static TheoryData<FilterOperator, string, string[]> StringOperatorsData => new()
        {
            { FilterOperator.Contains, "a", ["Alice", "Carol", "Dave"] },
            { FilterOperator.Contain, "a", ["Alice", "Carol", "Dave"] },
            { FilterOperator.StartsWith, "B", ["Bob"] },
            { FilterOperator.EndsWith, "e", ["Alice", "Dave"] }
        };

        public static TheoryData<FilterOperator, decimal, int[]> DecimalComparisonData => new()
        {
            { FilterOperator.Equal, 20.7m, [2] },
            { FilterOperator.NotEqual, 20.7m, [1, 3, 4] },
            { FilterOperator.GreaterThan, 20.7m, [3, 4] },
            { FilterOperator.LessThan, 30.2m, [1, 2] },
            { FilterOperator.GreaterThanOrEqual, 30.2m, [3, 4] },
            { FilterOperator.LessThanOrEqual, 20.7m, [1, 2] }
        };

        public static TheoryData<FilterOperator, DateTime, int[]> DateTimeComparisonData => new()
        {
            { FilterOperator.Equal, new DateTime(2025, 2, 1, 0, 0, 0), new[] { 2 } },
            { FilterOperator.GreaterThan, new DateTime(2025, 2, 1, 0, 0, 0), new[] { 3, 4 } },
            { FilterOperator.LessThan, new DateTime(2025, 3, 1, 0, 0, 0), new[] { 1, 2 } },
            { FilterOperator.GreaterThanOrEqual, new DateTime(2025, 3, 1, 0, 0, 0), new[] { 3, 4 } },
            { FilterOperator.LessThanOrEqual, new DateTime(2025, 2, 1, 0, 0, 0), new[] { 1, 2 } }
        };


        public static TheoryData<string, FilterOperator, object[], int[]> ArrayOperationsData => new()
        {
            { "Id", FilterOperator.In, new object[] { 1, 3 }, new[] { 1, 3 } },
            { "Id", FilterOperator.NotIn, new object[] { 1, 3 }, new[] { 2, 4 } },
            { "Age", FilterOperator.In, new object[] { 25, 35 }, new[] { 1, 3 } },
            { "Name", FilterOperator.In, new object[] { "Alice", "Bob" }, new[] { 1, 2 } }
        };

        [Theory]
        [MemberData(nameof(NullOrEmptyFiltersData))]
        public void ApplyFilters_NullOrEmpty_ReturnsAllData(string? filters, int expectedCount)
        {
            var result = TestData.ApplyFilters(filters);
            result.Count().ShouldBe(expectedCount);
        }

        [Theory]
        [MemberData(nameof(ComparisonOperatorsData))]
        public void ApplyFilters_NumericComparisons_WorksCorrectly(FilterOperator op, object value, int expectedCount)
        {
            var filter = new FilterCriteria { Field = "Age", Operator = op, Value = value };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Count().ShouldBe(expectedCount);
        }

        [Theory]
        [MemberData(nameof(StringOperatorsData))]
        public void ApplyFilters_StringOperations_WorksCorrectly(FilterOperator op, string value, string[] expectedNames)
        {
            var filter = new FilterCriteria { Field = "Name", Operator = op, Value = value };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Name).ShouldBe(expectedNames);
        }

        [Theory]
        [MemberData(nameof(DecimalComparisonData))]
        public void ApplyFilters_DecimalComparisons_WorksCorrectly(FilterOperator op, decimal value, int[] expectedIds)
        {
            var filter = new FilterCriteria { Field = "Score", Operator = op, Value = value };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(expectedIds);
        }

        [Theory]
        [MemberData(nameof(DateTimeComparisonData))]
        public void ApplyFilters_DateTimeComparisons_WorksCorrectly(FilterOperator op, DateTime value, int[] expectedIds)
        {
            var filter = new FilterCriteria { Field = "CreatedDate", Operator = op, Value = value };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(expectedIds);
        }

        [Theory]
        [MemberData(nameof(ArrayOperationsData))]
        public void ApplyFilters_ArrayOperations_WorksCorrectly(string field, FilterOperator op, object[] values, int[] expectedIds)
        {
            var filter = new FilterCriteria { Field = field, Operator = op, Value = values };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(expectedIds);
        }

        [Fact]
        public void ApplyFilters_BooleanEquals_WorksCorrectly()
        {
            var filter = new FilterCriteria { Field = "IsActive", Operator = FilterOperator.Equals, Value = true };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Count().ShouldBe(3);
            result.All(x => x.IsActive).ShouldBeTrue();
        }

        [Fact]
        public void ApplyFilters_IsNull_WorksCorrectly()
        {
            var filter = new FilterCriteria { Field = "Tags", Operator = FilterOperator.IsNull };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Single().Id.ShouldBe(3);
        }

        [Fact]
        public void ApplyFilters_IsNotNull_WorksCorrectly()
        {
            var filter = new FilterCriteria { Field = "Tags", Operator = FilterOperator.IsNotNull };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 4 });
        }

        [Fact]
        public void ApplyFilters_RangeWithOnlyMin_WorksCorrectly()
        {
            var filter = new FilterCriteria
            {
                Field = "Age",
                Operator = FilterOperator.Range,
                Value = new Dictionary<string, object> { ["min"] = 35 }
            };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(new[] { 3, 4 });
        }

        [Fact]
        public void ApplyFilters_RangeWithOnlyMax_WorksCorrectly()
        {
            var filter = new FilterCriteria
            {
                Field = "Age",
                Operator = FilterOperator.Range,
                Value = new Dictionary<string, object> { ["max"] = 30 }
            };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void ApplyFilters_ComplexMultipleFilters_WorksCorrectly()
        {
            var filters = new[]
            {
                new FilterCriteria { Field = "Age", Operator = FilterOperator.Range, Value = new Dictionary<string, object> { ["min"] = 25, ["max"] = 35 } },
                new FilterCriteria { Field = "IsActive", Operator = FilterOperator.Equals, Value = true },
                new FilterCriteria { Field = "Score", Operator = FilterOperator.GreaterThan, Value = 20m }
            };
            var result = TestData.ApplyFilters(SerializeFilters(filters));
            result.Select(x => x.Id).ShouldBe(new[] { 3 });
        }
    }
    #endregion

    #region Invalid Filter Tests
    public class InvalidFilterTests
    {
        public static TheoryData<string, object> InvalidValueTypesData => new()
        {
            { "Age", "not a number" },
            { "Score", "invalid decimal" },
            { "CreatedDate", "not a date" },
            { "IsActive", "not a boolean" }
        };

        [Theory]
        [MemberData(nameof(InvalidValueTypesData))]
        public void ApplyFilters_InvalidValueType_ReturnsEmptySet(string field, object value)
        {
            var filter = new FilterCriteria { Field = field, Operator = FilterOperator.Equals, Value = value };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("{\"min\": \"invalid\", \"max\": 35}", new[] { 1, 2, 3 })] // Age <= 35
        [InlineData("{\"min\": 30, \"max\": \"invalid\"}", new[] { 2, 3, 4 })] // Age >= 30
        [InlineData("{\"min\": \"invalid\", \"max\": \"invalid\"}", new[] { 1, 2, 3, 4 })] // No filtering
        public void ApplyFilters_InvalidRangeValues_AppliesCorrectly(string rangeJson, int[] expectedIds)
        {
            var filter = new FilterCriteria { Field = "Age", Operator = FilterOperator.Range, Value = JsonSerializer.Deserialize<object>(rangeJson) };
            var result = TestData.ApplyFilters(SerializeFilters(filter));
            result.Select(x => x.Id).ShouldBe(expectedIds);
        }

        [Fact]
        public void ApplyFilters_MultipleInvalidFilters_ReturnsEmptySet()
        {
            var filters = new[]
            {
                new FilterCriteria { Field = "", Operator = FilterOperator.Equals, Value = true },
                new FilterCriteria { Field = "InvalidField", Operator = FilterOperator.Equals, Value = true },
                new FilterCriteria { Field = "Age", Operator = FilterOperator.Equals, Value = "not a number" }
            };
            var result = TestData.ApplyFilters(SerializeFilters(filters));
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ApplyFilters_MixedValidAndInvalidFilters_AppliesValidOnes()
        {
            var filters = new[]
            {
                new FilterCriteria { Field = "Age", Operator = FilterOperator.GreaterThan, Value = 30 },
                new FilterCriteria { Field = "InvalidField", Operator = FilterOperator.Equals, Value = true },
                new FilterCriteria { Field = "IsActive", Operator = FilterOperator.Equals, Value = true }
            };
            var result = TestData.ApplyFilters(SerializeFilters(filters));
            result.Select(x => x.Id).ShouldBe(new[] { 3, 4 });
        }
    }
    #endregion

    #region Error Handling Tests
    public class ErrorHandlingTests
    {
        public static TheoryData<string> InvalidJsonData => new()
        {
            "{malformed json}",
            "[{incomplete json",
            "not json at all",
            "{\"invalid\": true}"
        };

        [Theory]
        [MemberData(nameof(InvalidJsonData))]
        public void ApplyFilters_InvalidJson_ThrowsArgumentException(string invalidJson)
        {
            var exception = Should.Throw<ArgumentException>(() =>
                TestData.ApplyFilters(invalidJson));

            exception.Message.ShouldStartWith("Invalid filters parameter:");
            exception.InnerException.ShouldBeOfType<JsonException>();
        }
    }
    #endregion
}
