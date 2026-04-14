# Database Configuration Guide

## Översikt

FloorballCoach stödjer nu flera databasproviders för enkel växling mellan lokal utveckling och molnbaserade lösningar.

## Supported Providers

- **SQLite** - Lokal databas (standard)
- **SQL Server / Azure SQL** - Microsoft molnlösning
- **PostgreSQL** - Open source, fungerar med Supabase, Railway, etc.
- **MySQL** - Open source alternativ

## Snabbstart - Lokal SQLite (Standard)

```csharp
// Detta är standardkonfigurationen - ingen ändring behövs
var config = DatabaseConfiguration.GetLocalSQLiteConfig();
var factory = new DbContextFactory(config);
var context = factory.CreateDbContext();
```

## Byta till Azure SQL Database

### Steg 1: Installera NuGet-paket
```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Steg 2: Konfigurera i App.xaml.cs eller MainWindow.xaml.cs

```csharp
// I App.xaml.cs eller där du initialiserar din app
var config = DatabaseConfiguration.GetAzureSqlConfig(
    server: "din-server.database.windows.net",
    database: "floorballcoach-db",
    username: "adminuser",
    password: "dittLösenord123!"
);

var factory = new DbContextFactory(config);
var dbManager = new DatabaseManager(factory);

// Initiera databasen
await dbManager.InitializeDatabaseAsync();

// Använd factory för att skapa repository
var context = factory.CreateDbContext();
var playerRepository = new PlayerRepository(context);
```

## Byta till PostgreSQL (Supabase/Railway)

### Steg 1: Installera NuGet-paket
```powershell
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### Steg 2: Konfigurera

```csharp
var config = DatabaseConfiguration.GetPostgreSqlConfig(
    host: "db.supabase.co",
    database: "postgres",
    username: "postgres",
    password: "dittLösenord",
    port: 5432
);

var factory = new DbContextFactory(config);
```

## Byta till MySQL

### Steg 1: Installera NuGet-paket
```powershell
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

### Steg 2: Konfigurera

```csharp
var config = DatabaseConfiguration.GetMySqlConfig(
    server: "mysql-server.com",
    database: "floorballcoach",
    username: "user",
    password: "password"
);

var factory = new DbContextFactory(config);
```

## Testa anslutningen

```csharp
var dbManager = new DatabaseManager(factory);
bool isConnected = await dbManager.TestConnectionAsync();

if (isConnected)
{
    Console.WriteLine("✓ Databas ansluten!");
    Console.WriteLine(dbManager.GetDatabaseInfo());
}
else
{
    Console.WriteLine("✗ Kunde inte ansluta till databasen");
}
```

## Miljövariabler (Rekommenderat för produktion)

För säkerhet, lagra känslig data i miljövariabler eller en konfigurationsfil:

```csharp
var config = DatabaseConfiguration.GetAzureSqlConfig(
    server: Environment.GetEnvironmentVariable("DB_SERVER") ?? "default-server",
    database: Environment.GetEnvironmentVariable("DB_NAME") ?? "floorballcoach",
    username: Environment.GetEnvironmentVariable("DB_USER") ?? "admin",
    password: Environment.GetEnvironmentVariable("DB_PASSWORD") ?? ""
);
```

## Best Practices

1. **Utveckling**: Använd SQLite lokalt
2. **Produktion**: Använd molndatabas (Azure SQL, PostgreSQL, etc.)
3. **Säkerhet**: Lagra aldrig lösenord i källkoden
4. **Connection Pooling**: Återanvänd DbContext via factory-pattern
5. **Migration**: Använd `DatabaseManager.MigrateDatabaseAsync()` för uppdateringar

## Exempel: Komplett setup i App.xaml.cs

```csharp
public partial class App : Application
{
    private IDbContextFactory? _dbContextFactory;
    public static IDbContextFactory DbFactory { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Välj konfiguration baserat på miljö
        DatabaseConfiguration config;
        
        #if DEBUG
            // Lokal SQLite för utveckling
            config = DatabaseConfiguration.GetLocalSQLiteConfig();
        #else
            // Azure SQL för produktion
            config = DatabaseConfiguration.GetAzureSqlConfig(
                server: Environment.GetEnvironmentVariable("AZURE_SQL_SERVER"),
                database: Environment.GetEnvironmentVariable("AZURE_SQL_DATABASE"),
                username: Environment.GetEnvironmentVariable("AZURE_SQL_USER"),
                password: Environment.GetEnvironmentVariable("AZURE_SQL_PASSWORD")
            );
        #endif

        _dbContextFactory = new DbContextFactory(config);
        DbFactory = _dbContextFactory;

        // Initiera databas
        var dbManager = new DatabaseManager(_dbContextFactory);
        await dbManager.InitializeDatabaseAsync();

        // Starta hauptfönster
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
```
