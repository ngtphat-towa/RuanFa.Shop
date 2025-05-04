using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Infrastructure.Data;
using Serilog;
using SQLitePCL;

namespace RuanFa.Shop.Application.SubcutaneousTests.Commons;

/// <summary>
/// In Subcutaneous tests we aren't testing integration with a real database,
/// so even if we weren't using SQLite we would use some in-memory database.
/// </summary>
public class SqliteTestDatabase : IDisposable
{
    static SqliteTestDatabase()
    {
        // Initialize SQLite provider
        Batteries.Init();
    }

    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");
        testDatabase.InitializeDatabase();
        return testDatabase;
    }

    public void InitializeDatabase()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(Connection)
            .LogTo(s => Log.Information(s)) // Log SQL output to the logger
            .EnableDetailedErrors(true) // Show detailed errors if any
            .EnableSensitiveDataLogging(true) // Log sensitive data for debugging purposes (caution)
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        Connection.Close();
        InitializeDatabase();
    }

    public void Dispose()
    {
        Connection.Close();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }
}
