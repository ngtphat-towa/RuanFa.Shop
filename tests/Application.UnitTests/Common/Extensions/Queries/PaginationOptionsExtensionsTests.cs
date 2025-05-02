using RuanFa.Shop.Application.Common.Extensions.Queries;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

public class PaginationOptionsExtensionsTests
{
    #region CreateAsync Tests
    public class CreateAsyncTests
    {
        [Fact]
        public async Task CreateAsync_NullIndexAndSize_UsesDefaultPageSize()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = await query.CreateAsync();

            result.Items.Count.ShouldBe(4); // Default page size (10) > total count (4)
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(10);
            result.TotalCount.ShouldBe(4);
            result.TotalPages.ShouldBe(1);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task CreateAsync_ValidIndexAndSize_PaginatesCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = await query.CreateAsync(index: 2, size: 2);

            result.Items.Count.ShouldBe(2);
            result.PageIndex.ShouldBe(2);
            result.PageSize.ShouldBe(2);
            result.TotalCount.ShouldBe(4);
            result.TotalPages.ShouldBe(2);
            result.HasPreviousPage.ShouldBeTrue();
            result.HasNextPage.ShouldBeFalse();
            result.Items.Select(x => x.Id).ShouldBe(new[] { 3, 4 }); // Second page
        }

        [Fact]
        public async Task CreateAsync_IndexLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
                query.CreateAsync(index: 0, size: 10));

            exception.ParamName.ShouldBe("index");
            exception.Message.ShouldContain("Page index must be greater than or equal to 1.");
        }

        [Fact]
        public async Task CreateAsync_SizeLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
                query.CreateAsync(index: 1, size: 0));

            exception.ParamName.ShouldBe("size");
            exception.Message.ShouldContain("Page size must be greater than or equal to 1.");
        }

        [Fact]
        public async Task CreateAsync_SizeExceedsMax_ThrowsArgumentOutOfRangeException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
                query.CreateAsync(index: 1, size: 101));

            exception.ParamName.ShouldBe("size");
            exception.Message.ShouldContain("Page size cannot exceed 100.");
        }

        [Fact]
        public async Task CreateAsync_EmptyQuery_ReturnsEmptyPaginatedList()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            var result = await query.CreateAsync(index: 1, size: 10);

            result.Items.ShouldBeEmpty();
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(10);
            result.TotalCount.ShouldBe(0);
            result.TotalPages.ShouldBe(0);
            result.HasPreviousPage.ShouldBeFalse();
            result.HasNextPage.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateAsync_CancellationRequested_ThrowsOperationCanceledException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            await Should.ThrowAsync<OperationCanceledException>(() =>
                query.CreateAsync(cancellationToken: cts.Token));
        }
    }
    #endregion
}
