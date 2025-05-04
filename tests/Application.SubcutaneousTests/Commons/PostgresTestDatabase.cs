using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using RuanFa.Shop.Infrastructure.Data;
using Serilog;
using System.Text.RegularExpressions;

namespace RuanFa.Shop.Application.SubcutaneousTests.Commons;

public class PostgresTestDatabase : IDisposable
{
    private const string DefaultConnectionString = "Server=localhost;Database=quingfa.eshop;Username=postgres;Password=123456789";
    private const string MasterDatabase = "postgres";
    private const string DatabaseName = "quingfa.eshop";

    public string ConnectionString { get; }
    public NpgsqlConnection Connection { get; private set; }
    public NpgsqlDataSource? DataSource { get; private set; }
    private readonly string _schemaName;

    public static PostgresTestDatabase CreateAndInitialize(string? connectionString = null)
    {
        var testDatabase = new PostgresTestDatabase(connectionString ?? DefaultConnectionString);
        testDatabase.InitializeDatabase();
        return testDatabase;
    }

    public PostgresTestDatabase(string connectionString)
    {
        ConnectionString = connectionString;
        Connection = new NpgsqlConnection(connectionString);
        _schemaName = $"test_{Guid.NewGuid():N}";
        ValidateSchemaName(_schemaName);
    }

    public void InitializeDatabase()
    {
        try
        {
            var masterConnectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = MasterDatabase
            }.ToString();

            using (var masterConnection = new NpgsqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                // Check if database exists
                const string checkDatabaseSql = "SELECT 1 FROM pg_database WHERE datname = @dbName";
                using (var checkCommand = masterConnection.CreateCommand())
                {
                    checkCommand.CommandText = checkDatabaseSql;
                    checkCommand.Parameters.AddWithValue("@dbName", DatabaseName);
                    var exists = checkCommand.ExecuteScalar() != null;
                    if (!exists)
                    {
                        // Create database if it doesn't exist
                        const string createDatabaseSql = "CREATE DATABASE \"quingfa.eshop\"";
                        using var createCommand = masterConnection.CreateCommand();
                        createCommand.CommandText = createDatabaseSql;
                        createCommand.ExecuteNonQuery();
                    }
                }
            }

            // Ensure connection is open
            EnsureConnectionIsOpen();

            // Create schema
            const string createSchemaSql = "CREATE SCHEMA IF NOT EXISTS @schemaName";
            using (var createSchemaCmd = Connection.CreateCommand())
            {
                createSchemaCmd.CommandText = createSchemaSql.Replace("@schemaName", EscapeIdentifier(_schemaName));
                createSchemaCmd.ExecuteNonQuery();
            }

            // Initialize EF Core context and set search path
            var options = CreateDbContextOptions();
            using var context = new ApplicationDbContext(options);
            SetSearchPath(context);
            context.Database.Migrate();
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize test database");
            throw;
        }
    }

    public void ResetDatabase()
    {
        try
        {
            EnsureConnectionIsOpen();
            ValidateSchemaName(_schemaName);

            // Drop and recreate schema
            string dropAndCreateSchemaSql = $"DROP SCHEMA IF EXISTS {EscapeIdentifier(_schemaName)} CASCADE; CREATE SCHEMA {EscapeIdentifier(_schemaName)};";
            using (var schemaCmd = Connection.CreateCommand())
            {
                schemaCmd.CommandText = dropAndCreateSchemaSql;
                schemaCmd.ExecuteNonQuery();
            }

            // Initialize with migrations
            var options = CreateDbContextOptions();
            using var context = new ApplicationDbContext(options);
            SetSearchPath(context);
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to reset test database");
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                ValidateSchemaName(_schemaName);
                string dropSchemaSql = $"DROP SCHEMA IF EXISTS {EscapeIdentifier(_schemaName)} CASCADE;";
                using (var dropCmd = Connection.CreateCommand())
                {
                    dropCmd.CommandText = dropSchemaSql;
                    dropCmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to drop test schema during cleanup");
        }
        finally
        {
            Connection?.Dispose();
            DataSource?.Dispose();
        }
    }

    private void ValidateSchemaName(string schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName) || !Regex.IsMatch(schemaName, @"^[a-zA-Z0-9_]+$"))
        {
            throw new ArgumentException("Invalid schema name. Schema name must contain only letters, numbers, or underscores.", nameof(schemaName));
        }
    }

    private void EnsureConnectionIsOpen()
    {
        if (Connection == null || Connection.State != System.Data.ConnectionState.Open)
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            DataSource = dataSourceBuilder.Build();
            Connection?.Dispose(); // Dispose of the old connection first
            Connection = DataSource.CreateConnection();
            Connection.Open();
        }
    }

    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(Connection)
            .UseSnakeCaseNamingConvention()
            .LogTo(s => Log.Information(s))
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;
    }

    private void SetSearchPath(ApplicationDbContext context)
    {
        string sql = $"SET search_path TO {EscapeIdentifier(_schemaName)}";
        context.Database.ExecuteSqlRaw(sql);
    }

    private string EscapeIdentifier(string identifier)
    {
        // Properly escape PostgreSQL identifier to prevent SQL injection
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }
}
