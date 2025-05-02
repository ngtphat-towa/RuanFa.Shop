using System.Linq.Expressions;
using RuanFa.Shop.Application.Common.Extensions.Queries;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

public class CombinedPaginationAndSortOptionsTests
{
    #region Combined Tests
    public class CombinedTests
    {
        [Fact]
        public async Task CreateAsync_WithApplySort_PaginatesAndSortsCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var sortedQuery = query.ApplySort(sortBy: "Age", sortDirection: "desc");
            var result = await sortedQuery.CreateAsync(index: 2, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(2);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(4);
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeTrue();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Age).ShouldBe(new[] { 30, 25 }); // Second page, sorted by Age DESC
        }

        [Fact]
        public async Task CreateAsync_WithDefaultSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var sortedQuery = query.ApplySort(sortBy: null);
            var result = await sortedQuery.CreateAsync(index: 1, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(4);
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeTrue();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 1, 2 }); // First page, sorted by Id ASC
        }
    }
    #endregion
    #region Single Property Combined Tests
    public class SinglePropertyCombinedTests
    {
        [Fact]
        public async Task CreateAsync_WithSearchAndSort_PaginatesAndSortsCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var propertyName = (string?)"Name";
            var filteredQuery = query.ApplySearch("o", propertyName); // Matches "Bob", "Carol"
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(2); // Bob and Carol
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 2, 3 }); // Bob (Age 30), Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithSearchAndPagination_EmptyResult()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var propertyName = (string?)"Name";
            var filteredQuery = query.ApplySearch("NonExistent", propertyName); // No matches
            var result = await filteredQuery.CreateAsync(index: 1, size: 2);

            result.Items.ShouldBeEmpty();
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithIntSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var propertyName = (string?)"Age";
            var filteredQuery = query.ApplySearch("30", propertyName); // Matches Bob
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(1); // Bob
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 2 }); // Bob (Age 30)
        }

        [Fact]
        public async Task CreateAsync_WithDateSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var propertyName = "CreatedDate";
            var filteredQuery = query.ApplySearch("2025-01-01", propertyName); // Matches Alice
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(1); // Alice
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 1 }); // Alice (Age 25)
        }
    }
    #endregion

    #region Multi Property Combined Tests
    public class MultiPropertyCombinedTests
    {
        [Fact]
        public async Task CreateAsync_WithMultiPropertySearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var filteredQuery = query.ApplySearch("o", new[] { "Name" }); // Matches "Bob", "Carol"
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "desc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(2); // Bob and Carol
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeTrue();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 3 }); // Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithAllPropertiesSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var filteredQuery = query.ApplySearch("o", (string[]?)null); // Matches "Bob", "Carol" on Name
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(2); // Bob and Carol
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 2, 3 }); // Bob (Age 30), Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithMultiPropertySearchAndPagination_EmptyResult()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var filteredQuery = query.ApplySearch("NonExistent", new[] { "Name" }); // No matches
            var result = await filteredQuery.CreateAsync(index: 1, size: 2);

            result.Items.ShouldBeEmpty();
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
        }
    }
    #endregion

    #region Property Selector Combined Tests
    public class PropertySelectorCombinedTests
    {
        [Fact]
        public async Task CreateAsync_WithPropertySelectorSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var filteredQuery = query.ApplySearch("o", selector); // Matches "Bob", "Carol"
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "desc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(2); // Bob and Carol
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeTrue();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 3 }); // Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithAllPropertiesSelectorSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var filteredQuery = query.ApplySearch("o", (Expression<Func<TestEntity, string>>[]?)null); // Matches "Bob", "Carol" on Name
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(2); // Bob and Carol
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 2, 3 }); // Bob (Age 30), Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithPropertySelectorSearchAndPagination_EmptyResult()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var filteredQuery = query.ApplySearch("NonExistent", selector); // No matches
            var result = await filteredQuery.CreateAsync(index: 1, size: 2);

            result.Items.ShouldBeEmpty();
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
        }
    }
    #endregion

    #region Predicate Combined Tests
    public class PredicateCombinedTests
    {
        [Fact]
        public async Task CreateAsync_WithPredicateSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 30;
            var filteredQuery = query.ApplySearch(predicate); // Matches Carol, Dave
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(2); // Carol and Dave
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeTrue();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 3 }); // Carol (Age 35)
        }

        [Fact]
        public async Task CreateAsync_WithComplexPredicateSearchAndSort_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 25 && x.Name.ToLower().Contains("o".ToLower());
            var filteredQuery = query.ApplySearch(predicate); // Should match only "Carol"
            var sortedQuery = filteredQuery.ApplySort(sortBy: "Age", sortDirection: "asc");
            var result = await sortedQuery.CreateAsync(index: 1, size: 1);

            result.Items.Count.ShouldBe(1);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(1);
            result.TotalCount.ShouldBe(2); 
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeTrue();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 2 });
        }

        [Fact]
        public async Task CreateAsync_WithPredicateSearchAndPagination_EmptyResult()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 50; // No matches
            var filteredQuery = query.ApplySearch(predicate);
            var result = await filteredQuery.CreateAsync(index: 1, size: 2);

            result.Items.ShouldBeEmpty();
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
        }
    }
    #endregion
}
