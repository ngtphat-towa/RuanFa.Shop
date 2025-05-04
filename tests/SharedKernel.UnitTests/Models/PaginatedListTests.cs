using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models;

public class PaginatedListTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTimeOffset ? CreatedDate { get; set; }
    }
    #region Constructor Tests
    public class ConstructorTests
    {
        [Fact]
        public void PaginatedList_ValidParameters_InitializesCorrectly()
        {
            var items = new[] { new TestEntity { Id = 1 } }.ToList();
            var paginatedList = new PaginatedList<TestEntity>(items, totalCount: 10, pageIndex: 2, pageSize: 5);

            paginatedList.Items.ShouldBeSameAs(items);
            paginatedList.TotalCount.ShouldBe(10);
            paginatedList.PageIndex.ShouldBe(2);
            paginatedList.PageSize.ShouldBe(5);
            paginatedList.TotalPages.ShouldBe(2); // Ceiling(10/5) = 2
            paginatedList.HasPreviousPage.ShouldBeTrue();
            paginatedList.HasNextPage.ShouldBeFalse();
        }

        [Fact]
        public void PaginatedList_NullItems_ThrowsArgumentNullException()
        {
            var exception = Should.Throw<ArgumentNullException>(() =>
                new PaginatedList<TestEntity>(null!, totalCount: 10, pageIndex: 1, pageSize: 10));

            exception.ParamName.ShouldBe("items");
        }

        [Fact]
        public void PaginatedList_NegativeTotalCount_ThrowsArgumentOutOfRangeException()
        {
            var items = new[] { new TestEntity() }.ToList();
            var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
                new PaginatedList<TestEntity>(items, totalCount: -1, pageIndex: 1, pageSize: 10));

            exception.ParamName.ShouldBe("totalCount");
            exception.Message.ShouldContain("Total count must be non-negative.");
        }

        [Fact]
        public void PaginatedList_ZeroPageIndex_ThrowsArgumentOutOfRangeException()
        {
            var items = new[] { new TestEntity() }.ToList();
            var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
                new PaginatedList<TestEntity>(items, totalCount: 10, pageIndex: 0, pageSize: 10));

            exception.ParamName.ShouldBe("pageIndex");
            exception.Message.ShouldContain("Page index must be greater than or equal to 1.");
        }

        [Fact]
        public void PaginatedList_ZeroPageSize_ThrowsArgumentOutOfRangeException()
        {
            var items = new[] { new TestEntity() }.ToList();
            var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
                new PaginatedList<TestEntity>(items, totalCount: 10, pageIndex: 1, pageSize: 0));

            exception.ParamName.ShouldBe("pageSize");
            exception.Message.ShouldContain("Page size must be greater than or equal to 1.");
        }

        [Fact]
        public void PaginatedList_EmptyCollection_SetsTotalPagesToZero()
        {
            var items = Array.Empty<TestEntity>().ToList();
            var paginatedList = new PaginatedList<TestEntity>(items, totalCount: 0, pageIndex: 1, pageSize: 10);

            paginatedList.TotalPages.ShouldBe(0);
            paginatedList.HasNextPage.ShouldBeFalse();
            paginatedList.HasPreviousPage.ShouldBeFalse();
        }
    }
    #endregion
}
