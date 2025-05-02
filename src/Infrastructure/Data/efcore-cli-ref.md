---

# 📘 EF Core CLI Commands for Quingfa Project

Assuming your project structure is as follows:

- **Infrastructure Project**: `src/Infrastructure/Infrastructure.csproj`
- **Startup Project**: `src/WebApi/WebApi.csproj`
- **DbContext**: `Infrastructure.Data.ApplicationDbContext`

Ensure that the EF Core CLI tools are installed globally:

```bash
dotnet tool install --global dotnet-ef
```

---

## 📦 Migrations Commands

### 1. Add a New Migration

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

**Options Explained**:

- `<MigrationName>`: Replace with your desired migration name.
- `--project`: Specifies the project containing the DbContext.
- `--startup-project`: Specifies the startup project to use.
- `--context`: Specifies the DbContext to use.
- `--configuration`: Defines the build configuration (e.g., Debug or Release).
- `--verbose`: Enables detailed output for debugging purposes.

### 2. Remove the Last Migration

```bash
dotnet ef migrations remove \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

Removes the last migration that was added but not yet applied to the database.

### 3. List All Migrations

```bash
dotnet ef migrations list \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

Displays a list of all migrations applied to the database.

### 4. Generate SQL Script from Migrations

```bash
dotnet ef migrations script \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

Generates a SQL script from the migrations.

---

## 🗄️ Database Commands

### 1. Update the Database

```bash
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

Applies any pending migrations to the database.

### 2. Drop the Database

```bash
dotnet ef database drop \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose \
  --force
```

Drops the database associated with the specified DbContext. The `--force` option skips the confirmation prompt.

---

## 🧰 DbContext Commands

### 1. Scaffold DbContext and Entities from an Existing Database

```bash
dotnet ef dbcontext scaffold "YourConnectionString" Microsoft.EntityFrameworkCore.SqlServer \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --output-dir Models \
  --configuration Debug \
  --verbose
```

**Options Explained**:

- `"YourConnectionString"`: Replace with your actual database connection string.
- `Microsoft.EntityFrameworkCore.SqlServer`: Specifies the database provider.
- `--output-dir`: Specifies the directory to place the generated models.

### 2. List Available DbContext Types

```bash
dotnet ef dbcontext list \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --configuration Debug \
  --verbose
```

Lists all DbContext types in the specified project.

### 3. Get Information About a DbContext

```bash
dotnet ef dbcontext info \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj \
  --context Infrastructure.Data.ApplicationDbContext \
  --configuration Debug \
  --verbose
```

Displays information about the specified DbContext.

---

## 📝 Notes

- **Project Structure**: Ensure that your `DbContext` and entity classes reside in the `src/Infrastructure` project, and that `src/WebApi` is configured as the startup project.
- **Configuration**: The `--configuration Debug` flag specifies that the Debug build configuration should be used. Adjust this if you're using a different build configuration.
- **Verbose Output**: Use the `--verbose` flag for detailed output during command execution.
