using RuanFa.Shop.Application.Common.Extensions.Queries;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

public class SortOptionsExtensionTests
{
    #region ApplySort Tests
    public class ApplySortTests
    {
        [Fact]
        public async Task ApplySort_ValidPropertyAndDirection_SortsCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySort(sortBy: "Name", sortDirection: "desc").ToList();

            result.Select(x => x.Name).ShouldBe(new[] { "Dave", "Carol", "Bob", "Alice" });
        }

        [Fact]
        public async Task ApplySort_NullSortBy_AppliesDefaultSortById()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySort(sortBy: null).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 }); // Ascending by Id
        }

        [Fact]
        public async Task ApplySort_InvalidProperty_ReturnsUnsortedQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySort(sortBy: "Invalid", sortDirection: "asc").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 }); // Original order
        }

        [Fact]
        public async Task ApplySort_InvalidSortDirection_ThrowsArgumentException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var exception = Should.Throw<ArgumentException>(() =>
                query.ApplySort(sortBy: "Name", sortDirection: "invalid"));

            exception.ParamName.ShouldBe("sortDirection");
            exception.Message.ShouldContain("Invalid sort direction 'invalid'. Must be 'asc' or 'desc'.");
        }

        [Fact]
        public async Task ApplySort_NullableProperty_SortsCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySort(sortBy: "CreatedDate", sortDirection: "asc").ToList();

            var expected = new DateTime ?[]
            {
                new DateTime(2025, 1, 1, 0, 0, 0),
                new DateTime(2025, 2, 1, 0, 0, 0),
                new DateTime(2025, 3, 1, 0, 0, 0),
                new DateTime(2025, 4, 1, 0, 0, 0)
            };
            result.Select(x => x.CreatedDate).ShouldBe(expected);
        }

        [Fact]
        public async Task ApplySort_EmptyQuery_ReturnsEmptyQuery()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            var result = query.ApplySort(sortBy: "Name", sortDirection: "asc").ToList();

            result.ShouldBeEmpty();
        }
    }
    #endregion
}
