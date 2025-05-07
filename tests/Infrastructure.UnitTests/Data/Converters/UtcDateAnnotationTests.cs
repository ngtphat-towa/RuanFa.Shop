using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Infrastructure.Data.Converters;

namespace RuanFa.Shop.Infrastructure.UnitTests.Data.Converters
{
    public class UtcDateAnnotationTests
    {
        private readonly DbContextOptions<TestDbContext> _opts;

        public UtcDateAnnotationTests()
        {
            // Each test gets a fresh in-memory database
            _opts = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Force model creation (so converters are wired up)
            using var init = new TestDbContext(_opts);
            init.Database.EnsureCreated();
        }

        [Theory]
        [InlineData("2025-05-03T15:30:00", DateTimeKind.Local)]
        [InlineData("2025-05-03T15:30:00", DateTimeKind.Unspecified)]
        [InlineData("2025-05-03T15:30:00", DateTimeKind.Utc)]
        public void DateTime_Properties_Roundtrip_As_Utc(string dateTimeIso, DateTimeKind incomingKind)
        {
            // Arrange: build a DateTime with the requested Kind
            var dt = DateTime.SpecifyKind(DateTime.Parse(dateTimeIso), incomingKind);

            var entity = new TestEntity
            {
                Id = 1,
                Dt = dt,
                DtNullable = dt,
                LocalDt = dt   // this property opts out of UTC conversion
            };

            // Act: save and re-load
            using (var ctx = new TestDbContext(_opts))
            {
                ctx.Entities.Add(entity);
                ctx.SaveChanges();
            }

            TestEntity fromDb;
            using (var ctx = new TestDbContext(_opts))
            {
                fromDb = ctx.Entities.Single();
            }

            // Assert: all non-opt-out DateTime fields are normalized to UTC
            fromDb.Dt.Kind.ShouldBe(DateTimeKind.Utc);
            fromDb.Dt.ShouldBe(dt.ToUniversalTime());

            fromDb.DtNullable.ShouldNotBeNull();
            fromDb.DtNullable!.Value.Kind.ShouldBe(DateTimeKind.Utc);
            fromDb.DtNullable.Value.ShouldBe(dt.ToUniversalTime());

            // Assert: the opted-out field preserves its original Kind
            fromDb.LocalDt.Kind.ShouldBe(incomingKind);
            if (incomingKind == DateTimeKind.Utc)
            {
                fromDb.LocalDt.ShouldBe(dt);
            }
        }

        [Theory]
        [InlineData("2025-05-03T15:30:00-05:00")]
        [InlineData("2025-05-03T15:30:00+01:00")]
        [InlineData("2025-05-03T15:30:00Z")]
        public void DateTimeOffset_Properties_Roundtrip_As_Utc(string offsetDateTimeIso)
        {
            // Arrange
            var dto = DateTimeOffset.Parse(offsetDateTimeIso);

            var entity = new TestEntity
            {
                Id = 2,
                Dto = dto,
                DtoNullable = dto
            };

            // Act: save and re-load
            using (var ctx = new TestDbContext(_opts))
            {
                ctx.Entities.Add(entity);
                ctx.SaveChanges();
            }

            TestEntity fromDb;
            using (var ctx = new TestDbContext(_opts))
            {
                fromDb = ctx.Entities.Single(e => e.Id == 2);
            }

            // Assert: all DateTimeOffset fields are normalized to UTC
            fromDb.Dto.Offset.ShouldBe(TimeSpan.Zero);
            fromDb.Dto.UtcDateTime.ShouldBe(dto.UtcDateTime);

            fromDb.DtoNullable.ShouldNotBeNull();
            fromDb.DtoNullable!.Value.Offset.ShouldBe(TimeSpan.Zero);
            fromDb.DtoNullable.Value.UtcDateTime.ShouldBe(dto.UtcDateTime);
        }

        [Fact]
        public void Null_DateTimeOffset_Properties_Handled_Correctly()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = 3,
                DtNullable = null,
                DtoNullable = null
            };

            // Act: save and re-load
            using (var ctx = new TestDbContext(_opts))
            {
                ctx.Entities.Add(entity);
                ctx.SaveChanges();
            }

            TestEntity fromDb;
            using (var ctx = new TestDbContext(_opts))
            {
                fromDb = ctx.Entities.Single(e => e.Id == 3);
            }

            // Assert: null values remain null
            fromDb.DtNullable.ShouldBeNull();
            fromDb.DtoNullable.ShouldBeNull();
        }

        [Fact]
        public void Explicitly_Opted_Out_Property_Is_Not_Converted()
        {
            // Arrange: a local DateTime for the opted-out property
            var localDt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            var e = new TestEntity { Id = 4, LocalDt = localDt };

            // Act
            using (var ctx = new TestDbContext(_opts))
            {
                ctx.Entities.Add(e);
                ctx.SaveChanges();
            }

            TestEntity fromDb;
            using (var ctx = new TestDbContext(_opts))
            {
                fromDb = ctx.Entities.Single(e => e.Id == 4);
            }

            // Assert: LocalDt was marked .IsUtc(false), so it must remain Local
            fromDb.LocalDt.Kind.ShouldBe(DateTimeKind.Local);
            fromDb.LocalDt.ShouldBe(localDt);
        }

        // --- Helper types for the test ---
        private class TestDbContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; } = null!;

            public TestDbContext(DbContextOptions<TestDbContext> opts)
                : base(opts) { }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                // Opt-out only for LocalDt
                builder.Entity<TestEntity>()
                       .Property(e => e.LocalDt)
                       .IsUtc(isUtc: false);

                // Apply your UTC converters to every other DateTime/DateTimeOffset
                builder.ApplyUtcDateTimeConverter();

                base.OnModelCreating(builder);
            }
        }

        private class TestEntity
        {
            public int Id { get; set; }

            public DateTime Dt { get; set; }
            public DateTime? DtNullable { get; set; }

            public DateTimeOffset Dto { get; set; }
            public DateTimeOffset? DtoNullable { get; set; }

            // This property is explicitly opted out via .IsUtc(false)
            public DateTime LocalDt { get; set; }
        }
    }
}
