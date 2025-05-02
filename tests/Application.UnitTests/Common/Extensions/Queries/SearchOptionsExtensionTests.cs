using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using System.Linq.Expressions;

namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

public class SearchOptionsExtensionTests
{
    #region Single Property Search Tests
    public class SinglePropertySearchTests
    {
        [Fact]
        public async Task Search_StringProperty_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("li", (string?)"Name").ToList(); // Matches "Alice"

            result.Select(x => x.Id).ShouldBe(new[] { 1 });
        }

        [Fact]
        public async Task Search_StringProperty_CaseInsensitive()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("ALICE", (string?)"Name").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1 });
        }

        [Fact]
        public async Task Search_StringProperty_NullSearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch(null, (string?)"Name").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_StringProperty_EmptySearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("", (string?)"Name").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_StringProperty_NullPropertyName_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("Alice", (string?)null).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1 });
            result.Select(x => x.Name).ShouldBe(new[] { "Alice" });
        }

        [Fact]
        public async Task Search_StringProperty_InvalidPropertyName_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("Alice", (string?)"Invalid").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_IntProperty_ExactMatch()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("30", (string?)"Age").ToList(); // Matches Bob

            result.Select(x => x.Id).ShouldBe(new[] { 2 });
        }

        [Fact]
        public async Task Search_IntProperty_InvalidTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("invalid", (string?)"Age").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_DateProperty_ExactMatch()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("2025-01-01", "CreatedDate").ToList(); // Matches Alice

            result.Select(x => x.Id).ShouldBe(new[] { 1 });
        }

        [Fact]
        public async Task Search_DateProperty_InvalidTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("invalid", (string?)"CreatedDate").ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_UnsupportedPropertyType_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("123m", (string?)"Id").ToList(); // Id is int, but not handled explicitly

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_EmptyQuery_ReturnsEmptyQuery()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            var result = query.ApplySearch("Alice", (string?)"Name").ToList();

            result.ShouldBeEmpty();
        }
    }
    #endregion

    #region Multi Property Search Tests
    public class MultiPropertySearchTests
    {
        [Fact]
        public async Task Search_MultipleStringProperties_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", new[] { "Name" }).ToList(); // Matches "Bob", "Carol"

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_AllStringProperties_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", (string[]?)null).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_NullSearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch(null, new[] { "Name" }).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_EmptySearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("", new[] { "Name" }).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_NullPropertyNames_ReturnsAllStringProperties()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", (string[]?)null).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_EmptyPropertyNames_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", new string[] { }).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_InvalidPropertyNames_SkipsInvalid()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", new[] { "Name", "Invalid", "Age" }).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_NoStringProperties_ReturnsOriginalQuery()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            await context.Database.EnsureCreatedAsync();
            context.AddRange(new[]
            {
                new NonStringEntity { Id = 1, Number = 10 },
                new NonStringEntity { Id = 2, Number = 20 }
            });
            await context.SaveChangesAsync();
            var query = context.Set<NonStringEntity>().AsQueryable();

            var result = query.ApplySearch("10", (string[]?)null).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public async Task Search_EmptyQuery_ReturnsEmptyQuery()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            var result = query.ApplySearch("o", new[] { "Name" }).ToList();

            result.ShouldBeEmpty();
        }
    }
    #endregion

    #region Property Selector Search Tests
    public class PropertySelectorSearchTests
    {
        [Fact]
        public async Task Search_PropertySelectors_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var result = query.ApplySearch("o", selector).ToList(); // Matches "Bob", "Carol"

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_PropertySelectors_MultipleSelectors_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector1 = x => x.Name;
            Expression<Func<TestEntity, string>> selector2 = x => x.Name + "Extra";
            var result = query.ApplySearch("o", selector1, selector2).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_PropertySelectors_NullSearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var result = query.ApplySearch(null, selector).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_PropertySelectors_EmptySearchTerm_ReturnsOriginalQuery()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var result = query.ApplySearch("", selector).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public async Task Search_PropertySelectors_NullSelectors_UsesAllStringProperties()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", (Expression<Func<TestEntity, string>>[]?)null).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_PropertySelectors_EmptySelectors_UsesAllStringProperties()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            var result = query.ApplySearch("o", new Expression<Func<TestEntity, string>>[] { }).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_PropertySelectors_InvalidSelector_SkipsInvalid()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, string>> validSelector = x => x.Name;
            Expression<Func<TestEntity, string>> invalidSelector = x => x.Age.ToString();
            var result = query.ApplySearch("o", validSelector, invalidSelector).ToList(); // Matches "Bob", "Carol" on Name

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
        }

        [Fact]
        public async Task Search_PropertySelectors_NoStringProperties_ReturnsOriginalQuery()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            await context.Database.EnsureCreatedAsync();
            context.AddRange(new[]
            {
                new NonStringEntity { Id = 1, Number = 10 },
                new NonStringEntity { Id = 2, Number = 20 }
            });
            await context.SaveChangesAsync();
            var query = context.Set<NonStringEntity>().AsQueryable();

            var result = query.ApplySearch("10", (Expression<Func<NonStringEntity, string>>[]?)null).ToList();

            result.Select(x => x.Id).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public async Task Search_PropertySelectors_EmptyQuery_ReturnsEmptyQuery()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            Expression<Func<TestEntity, string>> selector = x => x.Name;
            var result = query.ApplySearch("o", selector).ToList();

            result.ShouldBeEmpty();
        }
    }
    #endregion

    #region Predicate Search Tests
    public class PredicateSearchTests
    {
        [Fact]
        public async Task Search_Predicate_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 30;
            var result = query.ApplySearch(predicate).ToList(); // Matches Carol, Dave

            result.Select(x => x.Id).ShouldBe(new[] { 3, 4 });
        }

        [Fact]
        public async Task Search_Predicate_ComplexCondition_FiltersCorrectly()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 25 && x.Name.ToLower().Contains("o".ToLower());
            var result = query.ApplySearch(predicate).ToList(); // Should match only "Carol"

            result.Select(x => x.Id).ShouldBe(new[] { 2, 3 });
            result.Select(x => x.Name).ShouldBe(new[] { "Bob", "Carol" });
        }

        [Fact]
        public async Task Search_Predicate_NullPredicate_ThrowsArgumentNullException()
        {
            var query = await TestDataSetup.GetTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = null!;
            var exception = Should.Throw<ArgumentNullException>(() => query.ApplySearch(predicate).ToList());

            exception.ParamName.ShouldBe("predicate");
        }

        [Fact]
        public async Task Search_Predicate_EmptyQuery_ReturnsEmptyQuery()
        {
            var query = await TestDataSetup.GetEmptyTestDataQueryable();
            Expression<Func<TestEntity, bool>> predicate = x => x.Age > 30;
            var result = query.ApplySearch(predicate).ToList();

            result.ShouldBeEmpty();
        }
    }
    #endregion
}
